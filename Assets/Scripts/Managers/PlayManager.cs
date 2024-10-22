using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 玩家状态
/// </summary>
public enum PlayState
{
    SPECTING,
    ELECTED,
    DELETING // DEPRECATED Remove Blocks
}

/// <summary>
/// 游戏进程
/// </summary>
public enum ProgressState
{
    DAYTIME,
    NIGHT
}

/// <summary>
/// 资源种类
/// </summary>
public enum ResourceType
{
    GOLD = 0,
    FOOD = 1,
    METAL = 2
}

public class PlayManager : MonoBehaviour
{
    ////////////////////////
    // Placing
    [HideInInspector]
    public Queue<int> nextPieces;
    private Queue<int> nextBag;

    [HideInInspector]
    public PlayState playState;

    /// <summary>
    /// 方块预置
    /// </summary>
    public GameObject[] piecePrefabs;

    private PieceData[] pieceDatas;

    ////////////////////////
    // Previews

    [Header("Select Preview")]
    public GameObject selectedPrefab;
    public PieceData selectedData;
    public UnitType selectedType;

    [Header("Prefabs")]
    public GameObject playerPrefab;

    public GameObject previewInstance { get; private set; }

    private WorldManager worldManager;
    private HUDManager hudManager;
    private MissionManager missionManager;
    private EventDispatcher eventDispatcher;
    private TerrainUnitConfig unitConfig;
    private UICameraController uiCameraController;
    public uint pieceCount { get; private set; }


    private Vector3Int selectingPosition;
    /// <summary>
    /// 当前选择的方块是否能放置
    /// </summary>
    private bool slectingCanPlace = false;

    ////////////////////////
    // Game Progress

    /// <summary>
    /// 玩家持有的资源
    /// </summary>
    /// <value></value>
    public Dictionary<ResourceType, int> playerResources;
    public Dictionary<ResourceType, int> playerMaxResources;
    public ProgressState progressState { get; private set; }
    [Header("Game Progress")]
    public float dayTime;
    public float nightTime;
    private float curTime;
    public int dayCount = 0;

    [SerializeField]
    private bool progressStart = false;

    // 击溃的塔时的天数
    [ReadOnly]
    [SerializeField]
    private int[] towerDestroyed = new int[] { 9999, 9999 };

    public float goldRefillTime = 10f;
    private float curGoldRefillTime = 0;

    [Header("Enemy")]
    public GameObject enemyPrefab;
    public float enemySpawnDistance = 15;
    public GameObject enemyGiantPrefab;
    List<GameObject> enemies;


    private void Awake()
    {

        eventDispatcher = new EventDispatcher();
    }
    void Start()
    {

        worldManager = GameObject.Find("Map").GetComponent<WorldManager>();
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        missionManager = GameObject.Find("MissionManager").GetComponent<MissionManager>();
        unitConfig = GameObject.Find("UnitConfig").GetComponent<TerrainUnitConfig>();
        // init 
        this.pieceDatas = new PieceData[this.piecePrefabs.Length];
        for (var i = 0; i < this.piecePrefabs.Length; i++)
        {
            this.pieceDatas[i] = this.piecePrefabs[i].GetComponent<PieceData>();
        }
        nextPieces = new Queue<int>();

        this.playerResources = new Dictionary<ResourceType, int>();
        this.playerMaxResources = new Dictionary<ResourceType, int>();
        for (var i = 0; i < this.unitConfig.resourceConfig.Length; i++)
        {
            this.playerResources[unitConfig.resourceConfig[i].type] = unitConfig.resourceConfig[i].initialNumber;
            this.playerMaxResources[unitConfig.resourceConfig[i].type] = unitConfig.resourceConfig[i].maxNumber;
        }



        // defines
        enemies = new List<GameObject>();
        nextBag = new Queue<int>();
        pieceCount = 0;
        selectedType = UnitType.Grass;
        playState = PlayState.SPECTING;


        GameObject uicam = GameObject.Find("UICam");
        uicam.SetActive(false);
        uicam.SetActive(true);
        uiCameraController = uicam.GetComponent<UICameraController>();

        hudManager.UpdateResource(playerResources, playerMaxResources);

        BindListeners();
    }

    void BindListeners()
    {
        AddEventListener(PlayEventType.PLAYER_KILL_TOWER, () =>
        {
            if (towerDestroyed[0] == 9999)
            {
                towerDestroyed[0] = dayCount;
            }
            else if (towerDestroyed[1] == 9999)
            {
                towerDestroyed[1] = dayCount;
            }
        });
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) ButtonRotateClockwise();
        if (Input.GetKeyDown(KeyCode.E)) ButtonRotateCounterClockwise();

        if (playState == PlayState.ELECTED)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 防止误触
                if (Input.mousePosition.y > 100)
                {
                    if (slectingCanPlace)
                    {
                        StartCoroutine(SpawnPiece());
                        // event patch
                        SendEvent(PlayEventType.PLAYER_PLACE);
                        switch (selectedType)
                        {
                            case UnitType.Road: SendEvent(PlayEventType.PLAYER_PLACE_ROAD); break;
                            case UnitType.Grass: SendEvent(PlayEventType.PLAYER_PLACE_GRASS); break;
                            case UnitType.Factor: SendEvent(PlayEventType.PLAYER_PLACE_FACTORY); break;
                            case UnitType.Defend: SendEvent(PlayEventType.PLAYER_PLACE_DEFEND); break;
                            case UnitType.Storage: SendEvent(PlayEventType.PLAYER_PLACE_STORAGE); break;
                            default: break;
                        }

                        this.playState = PlayState.SPECTING;
                        // hide preview
                        if (previewInstance) Destroy(previewInstance);
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                this.playState = PlayState.SPECTING;
                if (previewInstance) Destroy(previewInstance);
            }
        }

        if (playState == PlayState.SPECTING)
        {

        }

    }
    public void StartProgress()
    {
        SendEvent(PlayEventType.GAME_ENTER_DAY);
        progressStart = true;
    }
    private void FixedUpdate()
    {
        // 更新进度

        float percent = curTime;
        if (progressStart && pieceCount > 0 && progressState == ProgressState.DAYTIME)
        {

            if (curTime < dayTime)
                curTime += Time.fixedDeltaTime;
            percent /= dayTime;

            hudManager.UpdateTimeBoard(percent);
            if (percent > 1)
            {

                /// do spawn monster
                // 数量由天数决定 
                // 考虑非线性
                // Lazer
                int enemyCount = (int)((RD.NextDouble() * 0.5 + 0.5) * Mathf.Pow(dayCount, 0.7f) + 1);
                Debug.Log("Spawn Lazer: " + enemyCount);
                // enemies = new GameObject[enemyCount];
                while (enemyCount-- > 0)
                {
                    // random position
                    float thita = (RD.NextFloat() * 2 * Mathf.PI);
                    // 1-1.5倍距离生成
                    float actualSpawnDistance = enemySpawnDistance * (1 + RD.NextFloat() * 0.5f);
                    Vector3 spawnpoint = new Vector3(enemySpawnDistance * Mathf.Sin(thita), 3, enemySpawnDistance * Mathf.Cos(thita));
                    enemies.Add(Instantiate(enemyPrefab, worldManager.playerInstance.transform.position + spawnpoint, Quaternion.identity));
                }
                // Giant
                enemyCount = (int)((RD.NextDouble() * 0.5 + 0.5) * Mathf.Max(0, Mathf.Pow(dayCount - towerDestroyed[0], 0.6f)));
                Debug.Log("Spawn Giant: " + enemyCount);
                while (enemyCount-- > 0)
                {
                    bool enemyposN = RD.NextInt(2) == 1;
                    // enemies.Add(enemyposN?)
                    enemies.Add(Instantiate(enemyGiantPrefab,
                        GameUtils.PositionToTranform(
                            enemyposN ?
                            worldManager.GetUnit(worldManager.towerpos1).position :
                            worldManager.GetUnit(worldManager.towerpos2).position
                        ) + Vector3.up * 3,
                        Quaternion.identity)
                    );
                }

                // state change
                SendEvent(PlayEventType.GAME_ENTER_SWITCH);
                SendEvent(PlayEventType.GAME_ENTER_NIGHT);
                progressState = ProgressState.NIGHT;
                curTime = 0;
                hudManager.UpdateTimeBoard(0);
            }
        }

        if (progressState == ProgressState.NIGHT)
        {

            // update UI
            if (curTime < nightTime)
                curTime += Time.fixedDeltaTime;
            percent /= nightTime;

            enemies.RemoveAll(g => g == null);
            if ( /*all monster killed*/ enemies.Count == 0 || percent > 1)
            {



                SendEvent(PlayEventType.GAME_ENTER_SWITCH);
                SendEvent(PlayEventType.GAME_ENTER_DAY);
                progressState = ProgressState.DAYTIME;
                curTime = 0;
                dayCount++;
            }

        }


        //UI update

        // 补充包
        if (nextPieces.Count < piecePrefabs.Length)
        {
            FillNextPiece();
        }

        // 恒时补充资源
        if (goldRefillTime > curGoldRefillTime)
        {
            curGoldRefillTime += Time.fixedDeltaTime;
        }
        else
        {
            curGoldRefillTime -= goldRefillTime;
            GainResource(ResourceType.GOLD);
        }
    }

    /// <summary>
    /// 生成单片walkable方块
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnPiece()
    {

        pieceCount++;
        this.nextPieces.Dequeue();
        //update resource
        for (int i = 0; i < unitConfig.GetConfig(this.selectedType).resourceNeeded.Length; i++)
            this.playerResources[(ResourceType)i] -= unitConfig.GetConfig(this.selectedType).resourceNeeded[i];
        hudManager.UpdateResource(playerResources, playerMaxResources);

        hudManager.Placed(this.selectedType);
        foreach (Vector3Int occ in this.selectedData.GetOccupy())
        {
            TerrainUnit targetTerrain = worldManager.GetUnit(selectingPosition.x + occ.x, selectingPosition.z + occ.z);
            if (targetTerrain != null) targetTerrain.SetType(this.selectedType); else continue;
            yield return new WaitForSeconds(0.1f);
        }
        // end piece
        this.selectedData.ResetRotate();
    }

    /// <summary>
    /// 7-Bag Algorithm
    /// https://tetris.fandom.com/wiki/Random_Generator
    /// </summary>
    void FillNextBag()
    {
        if (nextBag.Count <= piecePrefabs.Length)
        {
            //fill a LENGTH long blocks
            int[] cps = new int[piecePrefabs.Length];
            for (int i = piecePrefabs.Length - 1; i >= 0; i--)
            {
                cps[i] = i;
            }
            for (int i = piecePrefabs.Length - 1; i >= 0; i--)
            {
                int rndnum = (int)(RD.NextDouble() * i);
                int tmp = cps[i];
                cps[i] = cps[rndnum];
                cps[rndnum] = tmp;
            }
            for (int i = 0; i < piecePrefabs.Length; i++)
            {
                nextBag.Enqueue(cps[i]);
            }
        }

    }

    /// <summary>
    /// 补充队列
    /// </summary>
    void FillNextPiece()
    {
        FillNextBag();
        this.nextPieces.Enqueue(nextBag.Dequeue());

        this.selectedPrefab = piecePrefabs[this.nextPieces.Peek()];
        this.selectedData = pieceDatas[this.nextPieces.Peek()];

        UpdatePreview();

    }

    /// <summary>
    /// HUD:更新方块预览
    /// </summary>
    public void UpdatePreview()
    {
        // update preview
        // selectedPrefab
        uiCameraController.SetInstance(
            this.selectedPrefab,
            nextPieces.Count > 2 ? piecePrefabs[nextPieces.ElementAt(1)] : null,
            nextPieces.Count > 2 ? piecePrefabs[nextPieces.ElementAt(2)] : null
        );

    }

    /// <summary>
    /// 通过鼠标事件更新预览方块的位置，并计算可否放置(this.slectingCanPlace)
    /// </summary>
    /// <param name="unit"></param>
    public void UpdateSlectingPosition(TerrainUnit unit)
    {
        if (playState == PlayState.ELECTED)
        {
            selectingPosition = unit.position;

            if (previewInstance)
            { // update transform
                previewInstance.transform.position = new Vector3(selectingPosition.x + 0.5f, selectingPosition.y / 2.0f + 0.2f, selectingPosition.z + 0.5f);//  +offsets
                previewInstance.transform.rotation = Quaternion.Euler(0, -90 * selectedData.rotate, 0) * Quaternion.identity;
            }

            slectingCanPlace = true;

            // whether on save alttitude 
            foreach (Vector3Int occ in this.selectedData.GetOccupy())
            {
                TerrainUnit targetTerrain = worldManager.GetUnit(selectingPosition.x + occ.x, selectingPosition.z + occ.z);
                if (targetTerrain == null || targetTerrain.type != UnitType.Empty || targetTerrain.position.y != worldManager.GetUnit(selectingPosition.x, selectingPosition.z).position.y)
                {
                    slectingCanPlace = false;
                    break;
                }
            }

            if (slectingCanPlace)
            {
                // is first one 
                // 检查周边有无已放置
                // targetTerrain
                bool hasNeibour = false;
                foreach (Vector3Int occ in this.selectedData.GetOccupy())
                {
                    TerrainUnit targetTerrain = worldManager.GetUnit(selectingPosition.x + occ.x, selectingPosition.z + occ.z);
                    if (targetTerrain != null && worldManager.HasNeibour(selectingPosition.x + occ.x, selectingPosition.z + occ.z,
                     t => t == UnitType.Road || t == UnitType.Spawn
                     ))
                    {
                        hasNeibour = true;
                        break;
                    }
                }
                if (!hasNeibour)
                    slectingCanPlace = false;
            }
            // 染色
            for (int i = 0; i < previewInstance.transform.childCount; i++)
            {
                Material mt = previewInstance.transform.GetChild(i).GetComponent<MeshRenderer>().material;
                mt.SetColor(
                    "_Color",
                    TerrainUnit.GetColorByType(
                        (this.slectingCanPlace) ? this.selectedType : UnitType.Empty
                    )
                );
                mt.SetFloat(
                    "_GOpacity",
                    0.5f
                );
                mt.SetFloat(
                    "_RimRang",
                    0
                );
            }
        }
    }


    public bool CanGainResource(ResourceType type){
        return this.playerResources[type] < this.playerMaxResources[type];
    }
    /// <summary>
    /// 获得资源（立即）
    /// </summary>
    public bool GainResource(ResourceType type)
    {
        if (!this.playerResources.ContainsKey(type))
            this.playerResources.Add(type, 0);

        // check is full
        bool changed = false;
        if (this.playerResources[type] < this.playerMaxResources[type])
        {
            this.playerResources[type]++;
            // FIXED Warning! Do not use player's position to reset unit.
            hudManager.UpdateResource(playerResources, playerMaxResources);
            changed = true;
        }
        return changed;
    }
    public bool GainResource(ResourceType type, Vector3Int position)
    {
        if (GainResource(type))
        {
            Debug.Log("Gain resource at: " + position);
            worldManager.GetUnit(position.x, position.z).canProducing = true;
            return true;
        }
        return false;
    }
    public void IncreaseMaxResource(int num = 1)
    {
        List<ResourceType> keys = new List<ResourceType>();
        foreach (var item in playerMaxResources.Keys)
        {
            keys.Add(item);
        }
        foreach (var item in keys)
        {
            playerMaxResources[item] = Math.Max(0, playerMaxResources[item] + num);
            playerResources[item] = Math.Min(playerResources[item], playerMaxResources[item]);
        }
        hudManager.UpdateResource(playerResources, playerMaxResources);
    }
    public void DecreaseMaxResource(int num = 1)
    {
        IncreaseMaxResource(-num);

    }


    /// <summary>
    /// 进入放置状态
    /// </summary>
    public void ButtonSelected()
    {
        playState = PlayState.ELECTED;
        if (previewInstance) Destroy(previewInstance);
        previewInstance = Instantiate(this.selectedPrefab, selectingPosition, Quaternion.identity);
    }

    /// <summary>
    ///  顺时针旋转
    /// </summary>
    public void ButtonRotateClockwise()
    {
        this.selectedData.DoRotate(true);
        uiCameraController.DoRotate(true);
    }
    /// <summary>
    ///  逆时针旋转
    /// </summary>
    /// 
    public void ButtonRotateCounterClockwise()
    {
        this.selectedData.DoRotate(false);
        uiCameraController.DoRotate(false);
    }
    /// <summary>
    /// 选择类型
    /// </summary>
    public void ButtonSetUnitType(string t)
    {
        switch (t)
        {
            case "Road": this.selectedType = UnitType.Road; break;
            case "Grass": this.selectedType = UnitType.Grass; break;
            case "Factor": this.selectedType = UnitType.Factor; break;
            case "Defend": this.selectedType = UnitType.Defend; break;
            case "Storage": this.selectedType = UnitType.Storage; break;
            case "Absorb": this.selectedType = UnitType.Absorb; break;
            default: this.selectedType = UnitType.Grass; break;
        }
        ButtonSelected();
    }


    public void AddEventListener(PlayEventType type, Action callback)
    {
        eventDispatcher.AddEventListener(type, callback);
    }
    public void RemoveEventListener(PlayEventType type, Action callback)
    {
        eventDispatcher.RemoveEventListener(type, callback);
    }
    public void SendEvent(PlayEventType type)
    {
        missionManager.AckEvent(type);
        eventDispatcher.SendEvent(type);
    }
}
