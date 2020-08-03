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
                rst.Add(item + Vector3Int.zero + Vector3.right * inclusion);
                rst.Add(item + new Vector3Int(0, 0, 1) + Vector3.right * inclusion);
            }
            if (!filter[1 + item.x + 1, 1 + item.z + 0])// DOWN
            {
                rst.Add(item + new Vector3Int(1, 0, 0) - Vector3.right * inclusion);
                rst.Add(item + new Vector3Int(1, 0, 1) - Vector3.right * inclusion);

            }
            if (!filter[1 + item.x + 0, 1 + item.z + -1])// LEFT
            {
                rst.Add(item + Vector3Int.zero + Vector3.forward * inclusion);
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

}