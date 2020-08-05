using System.Net;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BuffEffectManager : MonoBehaviour
{

    // 边界缩放
    public static float inclusion = 0.01f;
    public static List<Vector3> GetLineByVectors(List<Vector3Int> units)
    {

        var rst = new List<Vector3>();

        var filter = new bool[WorldManager._size.x + 2, WorldManager._size.y + 2];

        foreach (var item in units)
        {
            filter[item.x + 1, item.z + 1] = true;
        }

        foreach (var item in units)
        {
            if (!filter[1 + item.x + -1, 1 + item.z + 0])// UP
            {
                rst.Add(item +  Vector3.right * inclusion);
                rst.Add(item + new Vector3Int(0, 0, 1) + Vector3.right * inclusion);
            }
            if (!filter[1 + item.x + 1, 1 + item.z + 0])// DOWN
            {
                rst.Add(item + new Vector3Int(1, 0, 0) - Vector3.right * inclusion);
                rst.Add(item + new Vector3Int(1, 0, 1) - Vector3.right * inclusion);

            }
            if (!filter[1 + item.x + 0, 1 + item.z + -1])// LEFT
            {
                rst.Add(item + Vector3.forward * inclusion);
                rst.Add(item + new Vector3Int(1, 0, 0) + Vector3.forward * inclusion);

            }
            if (!filter[1 + item.x + 0, 1 + item.z + 1])// RIGHT
            {
                rst.Add(item + new Vector3Int(0, 0, 1) - Vector3.forward * inclusion);
                rst.Add(item + new Vector3Int(1, 0, 1) - Vector3.forward * inclusion);
            }
        }
        return rst;
    }

    public GameObject buffPrefab;
    public Queue<GameObject> buffInstances;

    private WorldManager worldManager;
    private void Awake()
    {
        worldManager = GameObject.Find("Map").GetComponent<WorldManager>();
        buffInstances = new Queue<GameObject>();
    }
    public void RefreshBuff()
    {
        while (buffInstances.Count > 0)
        {
            Destroy(buffInstances.Dequeue());
        }
        if (worldManager)
        {
            bool[,] visited = new bool[worldManager.size.x, worldManager.size.y];
            for (int _x = 0; _x < worldManager.size.x; _x++)
            {
                for (int _z = 0; _z < worldManager.size.y; _z++)
                {
                    if (!visited[_x, _z] && worldManager.GetUnit(_x, _z).localBuff != UnitBuffType.NONE)
                    {
                        visited[_x, _z] = true;
                        var buffRange = worldManager.SpreadBFS(new Vector3Int(_x, 0, _z),
                        (me, him) => me.localBuff == him.localBuff,
                        p =>
                        {
                            visited[p.x, p.z] = true;
                            return true;
                        });
                        GameObject buffInstance = Instantiate(buffPrefab, null);
                        buffInstance.GetComponent<BuffEffect>().LoadMeshByBlocks(buffRange, TerrainUnit.GetColorByBuffType(worldManager.GetUnit(_x, _z).localBuff));
                        buffInstances.Enqueue(buffInstance);
                    }
                }
            }
        }
    }
}