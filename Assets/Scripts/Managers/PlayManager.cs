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
        playState = PlayState.SPECTING;
        worldManager = GameObject.Find("Map").GetComponent<WorldManager>();
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
                                // canPlace = false;
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
                            this.nextPieces.Dequeue();
                            this.playState = PlayState.SPECTING;
                        }
                    }
                }
            }
        }
        if (nextPieces.Count < 7)
        {
            FillNextPiece();
        }
    }


    void FillNextPiece()
    {
        this.nextPieces.Enqueue((int)(RD.NextDouble() * piecePrefabs.GetLength(0)));

        this.selectedPrefab = piecePrefabs[this.nextPieces.Peek()];
        this.selectedData = pieceDatas[this.nextPieces.Peek()];
        
        UpdatePreview();
        
    }

    public void UpdatePreview(){
        // update preview
        // selectedPrefab


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
    }
    /// <summary>
    ///  逆时针旋转
    /// </summary>
    public void ButtonRotateCounterClockwise()
    {
        this.selectedData.DoRotate(false);
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