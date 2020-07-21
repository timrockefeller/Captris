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

            // show preview

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.collider.tag == "Terrain")
                    {
                        bool canPlace = true;
                        Vector3Int t = new Vector3Int((int)hitInfo.transform.position.x, (int)hitInfo.transform.position.y, (int)hitInfo.transform.position.z);
                        // check every block
                        foreach (Vector3Int occ in this.selectedData.GetOccupy())
                        {
                            TerrainUnit targetTerrain = worldManager.map[t.x + occ.x, t.z + occ.z];
                            if (targetTerrain.type != UnitType.Empty)
                            {
                                canPlace = false;
                                break;
                            }
                        }

                        // not first piece
                        // TODO : 检查周边有无已放置 （除第一块）
                        if (pieceCount != 0)
                        {

                        }else{
                            //generate player
                            Instantiate(playerPrefab,t+new Vector3(0.5f,5,0.5f),Quaternion.identity);
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
        if (playState == PlayState.SPECTING)
        {

            /// <summary>
            /// 预览方块
            /// </summary>
            /// <returns></returns>
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
}