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


    private WorldManager worldManager;
    private UICameraController uiCameraController;
    private uint pieceCount;

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
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo = new RaycastHit();

            // TODO show preview
            // if (Physics.Raycast(_ray, out hitInfo))
            // {
            //     if (hitInfo.collider.tag == "Terrain")
            //     {
            //         Vector3Int t = new Vector3Int((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.y, (int)hitInfo.transform.position.z);

            //     }
            // }

            if (Input.GetMouseButtonDown(0))
            {


                if (Physics.Raycast(_ray, out hitInfo))
                {
                    if (hitInfo.collider.tag == "Terrain")
                    {
                        bool canPlace = true;
                        Vector3Int t = new Vector3Int((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.y, (int)hitInfo.transform.position.z);
                        // check every block
                        foreach (Vector3Int occ in this.selectedData.GetOccupy())
                        {
                            TerrainUnit targetTerrain = worldManager.map[t.x + occ.x, t.z + occ.z];
                            if (targetTerrain.type != UnitType.Empty
                            || targetTerrain.position.y != worldManager.map[t.x, t.z].position.y)
                            {
                                canPlace = false;
                                break;
                            }
                        }

                        if (canPlace)
                        {
                            // not first piece
                            // TODO : 检查周边有无已放置 （除第一块）
                            if (pieceCount != 0)
                            {
                                // targetTerrain
                                bool hasNeibour = false;
                                foreach (Vector3Int occ in this.selectedData.GetOccupy())
                                {
                                    TerrainUnit targetTerrain = worldManager.map[t.x + occ.x, t.z + occ.z];
                                    if (worldManager.HasNeibour(t.x + occ.x, t.z + occ.z))
                                    {
                                        hasNeibour = true;
                                        break;
                                    }
                                }
                                if (!hasNeibour)
                                    canPlace = false;
                            }
                            else
                            {
                                //generate player
                                Instantiate(playerPrefab, t + new Vector3(0.5f, 5, 0.5f), Quaternion.identity);
                            }
                        }

                        if (canPlace)
                        {
                            foreach (Vector3Int occ in this.selectedData.GetOccupy())
                            {
                                TerrainUnit targetTerrain = worldManager.map[t.x + occ.x, t.z + occ.z];
                                targetTerrain.SetType(this.selectedType);
                            }
                            // end piece
                            pieceCount++;
                            this.selectedData.ResetRotate();
                            this.nextPieces.Dequeue();
                            this.playState = PlayState.SPECTING;
                        }
                    }
                }
            }
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
    /// 进入放置状态
    /// </summary>
    public void ButtonSelected()
    {
        playState = PlayState.ELECTED;
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
    }
}