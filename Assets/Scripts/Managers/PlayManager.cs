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
    DELETING // TODO
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
    private TerrainUnitConfig unitConfig;
    private UICameraController uiCameraController;
    public uint pieceCount { get; private set; }


    private Vector3Int selectingPosition;
    private bool slectingCanPlace = false;

    ////////////////////////
    // Game Progress

    /// <summary>
    /// 玩家持有的资源
    /// </summary>
    /// <value></value>
    public Dictionary<ResourceType, int> playerResources;

    public ProgressState progressState { get; private set; }
    [Header("Game Progress")]
    public float dayTime;
    public float nightTime;
    private float curTime;

    void Start()
    {
        worldManager = GameObject.Find("Map").GetComponent<WorldManager>();
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        unitConfig = GameObject.Find("UnitConfig").GetComponent<TerrainUnitConfig>();

        // init 
        this.pieceDatas = new PieceData[this.piecePrefabs.Length];
        for (var i = 0; i < this.piecePrefabs.Length; i++)
        {
            this.pieceDatas[i] = this.piecePrefabs[i].GetComponent<PieceData>();
        }
        nextPieces = new Queue<int>();

        this.playerResources = new Dictionary<ResourceType, int>();
        for (var i = 0; i < this.unitConfig.resourceConfig.Length; i++)
        {
            this.playerResources[unitConfig.resourceConfig[i].type] = unitConfig.resourceConfig[i].initialNumber;
        }



        // defines
        nextBag = new Queue<int>();
        pieceCount = 0;
        selectedType = UnitType.Grass;
        playState = PlayState.SPECTING;


        GameObject uicam = GameObject.Find("UICam");
        uicam.SetActive(false);
        uicam.SetActive(true);
        uiCameraController = uicam.GetComponent<UICameraController>();

        hudManager.UpdateResource(playerResources);
    }



    void Update()
    {
        if (playState == PlayState.ELECTED)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (slectingCanPlace)
                {
                    StartCoroutine("SpawnPiece");

                    this.playState = PlayState.SPECTING;
                    // hide preview
                    if (previewInstance) Destroy(previewInstance);
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

    private void FixedUpdate()
    {
        // 更新进度

        float percent = curTime;
        if (pieceCount > 0 && progressState == ProgressState.DAYTIME)
        {

            if (curTime < dayTime)
                curTime += Time.fixedDeltaTime;
            percent /= dayTime;
            hudManager.UpdateTimeBoard(percent);
        }

        if (progressState == ProgressState.NIGHT)
        {

            // TODO spawn monsters

        }
        //UI update
        if (percent > 1)
        {
            progressState = ProgressState.NIGHT;
        }
        // 补充包
        if (nextPieces.Count < piecePrefabs.Length)
        {
            FillNextPiece();
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
        hudManager.UpdateResource(playerResources);

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
                    if (targetTerrain != null && worldManager.HasNeibour(selectingPosition.x + occ.x, selectingPosition.z + occ.z))
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

    /// <summary>
    /// 获得资源（立即）
    /// </summary>
    public void GainResource(ResourceType type)
    {
        // TODO resource list
        if (!this.playerResources.ContainsKey(type))
            this.playerResources.Add(type, 0);
        this.playerResources[type]++;
        worldManager.GetUnit((int)worldManager.playerInstance.transform.position.x, (int)worldManager.playerInstance.transform.position.z).canProducing = true;
        hudManager.UpdateResource(playerResources);
        // throw new System.NotImplementedException();
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
            default: this.selectedType = UnitType.Grass; break;
        }
        ButtonSelected();
    }
}