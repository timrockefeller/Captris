using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PieceType
{
    O, I, J, L, Z, S, T
}

public class PieceData : MonoBehaviour
{
    public List<Vector3Int> occupy;

    public PieceType pieceType;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="isClockwise">true为顺时针，false为逆时针</param>
    public void DoRotate(bool isClockwise)
    {
        foreach (var occ in this.occupy)
        {
            occ.Set((isClockwise ? -1 : 1) * occ.z, 0, (isClockwise ? 1 : -1) * occ.x);
        }
    }
    
    void Start()
    {
        occupy = new List<Vector3Int>();
        switch (this.pieceType)
        {

            case PieceType.O:
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(-1, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, -1));
                occupy.Insert(occupy.Count, new Vector3Int(-1, 0, -1));
                break;
            case PieceType.I:
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(-1, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(1, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(2, 0, 0));
                break;
            case PieceType.T:
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(1, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 1));
                occupy.Insert(occupy.Count, new Vector3Int(-1, 0, 0));
                break;
            default:
                break;
        }
    }

}
