using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayState
{
    SPECTING,
    ELECTED,
}
public class PlayManager : MonoBehaviour
{
    [HideInInspector]
    public Queue<int> nextPieces;
    private Queue<int> nextBag;

    public PlayState playState;

    /// <summary>
    /// 方块预置
    /// </summary>
    public GameObject[] piecePrefabs;

    private PieceData[] pieceDatas;




    [Header("Select Preview")]
    public GameObject selectedPrefab;
    public PieceData selectedData;
    public UnitType selectedType;

    [Header("Prefabs")]
    public GameObject playerPrefab;

    public GameObject previewInstance { get; private set; }

    private WorldManager worldManager;
    private UICameraController uiCameraController;
    public uint pieceCount { get; private set; }


    private Vector3Int selectingPosition;
    private bool slectingCanPlace = false;

    void Start()
    {
        pieceCount = 0;
        selectedType = UnitType.Grass;
        this.pieceDatas = new PieceData[this.piecePrefabs.Length];
        for (var i = 0; i < this.piecePrefabs.Length; i++)
        {
            this.pieceDatas[i] = this.piecePrefabs[i].GetComponent<PieceData>();
        }
        nextPieces = new Queue<int>();
        nextBag = new Queue<int>();
        playState = PlayState.SPECTING;

        worldManager = GameObject.Find("Map").GetComponent<WorldManager>();
        GameObject uicam = GameObject.Find("UICam");
        uicam.SetActive(false);
        uicam.SetActive(true);
        uiCameraController = uicam.GetComponent<UICameraController>();
    }

    void Update()
    {
        if (playState == PlayState.ELECTED)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (slectingCanPlace)
                {
                    if (pieceCount == 0)
                    {
                        //generate player
                        Instantiate(playerPrefab, selectingPosition + new Vector3(0.5f, 5, 0.5f), Quaternion.identity);
                    }
                    foreach (Vector3Int occ in this.selectedData.GetOccupy())
                    {
                        TerrainUnit targetTerrain = worldManager.map[selectingPosition.x + occ.x, selectingPosition.z + occ.z];
                        targetTerrain.SetType(this.selectedType);
                    }
                    // end piece
                    pieceCount++;
                    this.selectedData.ResetRotate();
                    this.nextPieces.Dequeue();
                    this.playState = PlayState.SPECTING;
                    // hide preview
                    if (previewInstance) Destroy(previewInstance);
                }
            }
            // show preview
        }

        if (nextPieces.Count < piecePrefabs.Length)
        {
            FillNextPiece();
        }
    }

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
    void FillNextPiece()
    {
        FillNextBag();
        this.nextPieces.Enqueue(nextBag.Dequeue());

        this.selectedPrefab = piecePrefabs[this.nextPieces.Peek()];
        this.selectedData = pieceDatas[this.nextPieces.Peek()];

        UpdatePreview();

    }

    public void UpdatePreview()
    {
        // update preview
        // selectedPrefab
        uiCameraController.SetInstance(this.selectedPrefab);

    }


    /// <summary>
    /// 更新预览方块的位置，并计算可否放置
    /// </summary>
    /// <param name="unit"></param>
    public void UpdateSlectingPosition(TerrainUnit unit)
    {
        if (playState == PlayState.ELECTED)
        {
            selectingPosition = unit.position;

            if (previewInstance)
            { // update transform
                previewInstance.transform.position = new Vector3(selectingPosition.x + 0.5f, selectingPosition.y / 2.0f + 0.2f, selectingPosition.z + 0.5f);// TODO +offsets
                previewInstance.transform.rotation = Quaternion.Euler(0, -90 * selectedData.rotate, 0) * Quaternion.identity;
            }

            slectingCanPlace = true;
            foreach (Vector3Int occ in this.selectedData.GetOccupy())
            {
                TerrainUnit targetTerrain = worldManager.map[selectingPosition.x + occ.x, selectingPosition.z + occ.z];
                if (targetTerrain.type != UnitType.Empty
                || targetTerrain.position.y != worldManager.map[selectingPosition.x, selectingPosition.z].position.y)
                {
                    slectingCanPlace = false;
                    break;
                }
            }

            if (slectingCanPlace)
            {
                // is first one (TODO :spawn point)
                if (pieceCount != 0)
                {
                    // 检查周边有无已放置
                    // targetTerrain
                    bool hasNeibour = false;
                    foreach (Vector3Int occ in this.selectedData.GetOccupy())
                    {
                        TerrainUnit targetTerrain = worldManager.map[selectingPosition.x + occ.x, selectingPosition.z + occ.z];
                        if (worldManager.HasNeibour(selectingPosition.x + occ.x, selectingPosition.z + occ.z))
                        {
                            hasNeibour = true;
                            break;
                        }
                    }
                    if (!hasNeibour)
                        slectingCanPlace = false;
                }
            }

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
    /// 进入放置状态
    /// </summary>
    public void ButtonSelected()
    {
        playState = PlayState.ELECTED;

        if (previewInstance) Destroy(previewInstance);
        previewInstance = Instantiate(this.selectedPrefab, selectingPosition, Quaternion.identity);// TODO +offsets

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
            case "Grass": this.selectedType = UnitType.Grass; break;
            case "Factor": this.selectedType = UnitType.Factor; break;
            case "Defend": this.selectedType = UnitType.Defend; break;
            case "House": this.selectedType = UnitType.House; break;

            default: this.selectedType = UnitType.Grass; break;
        }
        ButtonSelected();
    }
}