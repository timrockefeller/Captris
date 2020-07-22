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

    private int rotate = 0;

    public void DoRotate(bool isClockwise)
    {
        if (isClockwise) rotate = (rotate + 1) % 4;
        else rotate = (rotate + 3) % 4;
    }
    public void ResetRotate()
    {
        rotate = 0;
    }

    public IEnumerable<Vector3Int> GetOccupy()
    {
        for (int i = 0; i < this.occupy.Count; i++)
        {
            if (rotate == 0) yield return this.occupy[i];
            if (rotate == 1 || rotate == 3) yield return new Vector3Int((rotate == 1 ? -1 : 1) * this.occupy[i].z,
             0,
             (rotate == 1 ? 1 : -1) * this.occupy[i].x);
            if (rotate == 2) yield return new Vector3Int(-this.occupy[i].x, 0, -this.occupy[i].z);
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
            case PieceType.J:
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(1, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 1));
                occupy.Insert(occupy.Count, new Vector3Int(2, 0, 0));
                break;
            case PieceType.L:
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(1, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, -1));
                occupy.Insert(occupy.Count, new Vector3Int(2, 0, 0));
                break;
            case PieceType.Z:
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(1, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, -1));
                occupy.Insert(occupy.Count, new Vector3Int(1, 0, 1));
                break;
            case PieceType.S:
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(1, 0, 0));
                occupy.Insert(occupy.Count, new Vector3Int(0, 0, -1));
                occupy.Insert(occupy.Count, new Vector3Int(-1, 0, -1));
                break;
            default:
                break;
        }
    }

}
