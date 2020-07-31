using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BuffEffectManager : MonoBehaviour
{
    public static List<Vector3Int> GetLineByVectors(List<Vector3Int> units)
    {

        var rst = new List<Vector3Int>();
        // int minX = 10000, minY = 10000, maxX = -1, maxY = -1;

        var filter = new bool[WorldManager._size.x + 2, WorldManager._size.y + 2];

        foreach (var item in units)
        {
            filter[item.x + 1, item.z + 1] = true;
            // minX = Math.Min(item.x, minX);
            // minY = Math.Min(item.z, minY);
            // maxX = Math.Max(item.x, maxX);
            // maxY = Math.Max(item.z, maxY);
        }

        foreach (var item in units)
        {
            if (!filter[1 + item.x + -1, 1 + item.z + 0])// UP
            {
                rst.Add(item + Vector3Int.zero);
                rst.Add(item + new Vector3Int(0, 0, 1));
            }
            if (!filter[1 + item.x + 1, 1 + item.z + 0])// DOWN
            {
                rst.Add(item + new Vector3Int(1, 0, 0));
                rst.Add(item + new Vector3Int(1, 0, 1));

            }
            if (!filter[1 + item.x + 0, 1 + item.z + -1])// LEFT
            {
                rst.Add(item + Vector3Int.zero);
                rst.Add(item + new Vector3Int(1, 0, 0));

            }
            if (!filter[1 + item.x + 0, 1 + item.z + 1])// RIGHT
            {
                rst.Add(item + new Vector3Int(0, 0, 1));
                rst.Add(item + new Vector3Int(1, 0, 1));
            }
        }
        return rst;
    }

}