using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tower_Eye : MonoBehaviour
{
    public List<TerrainUnit> aimList;
    private WorldManager worldManager;
    private PlayManager playManager;
    public bool playerComming;

    // Start is called before the first frame update
    void Start()
    {
        worldManager = GameObject.Find("Map").GetComponent<WorldManager>();
        playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();

        aimList = new List<TerrainUnit>();

        foreach (var iem in worldManager.SpreadBFS(Vector3Int.FloorToInt(transform.position), (a, b) => (a.transform.position - b.transform.position).magnitude<3))
        {
            aimList.Add(worldManager.GetUnit(iem));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Towereye triggered a " + other.tag);
        if (other.tag == "Player")
        {
            playerComming = true;
            playManager.SendEvent(PlayEventType.PLAYER_REACHED);
        }
    }
}
