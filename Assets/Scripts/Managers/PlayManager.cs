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

    public Queue<GameObject> nextPieces;


    public PlayState playState;

    public GameObject[] piecePrefabs;

    public GameObject selectedPrefab;

private WorldManager worldManager;
    void Start()
    {
        nextPieces = new Queue<GameObject>();
        playState = PlayState.SPECTING;
        worldManager = GameObject.Find("Map").GetComponent<WorldManager>();
    }

    void Update()
    {
        if (playState == PlayState.ELECTED)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.collider.tag == "Terrain")
                    {
                        
                        GameObject target = worldManager.map[(int)hitInfo.transform.position.x,(int)hitInfo.transform.position.z];
                        
                        if(target.GetComponent<TerrainUnit>().type==UnitType.Empty){
                            // TODO check every block
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
        this.nextPieces.Enqueue(piecePrefabs[(int)(RD.NextDouble() * piecePrefabs.GetLength(0))]);
    }


    public void ButtonSelected(){
        this.selectedPrefab = this.nextPieces.Peek();
    }
}