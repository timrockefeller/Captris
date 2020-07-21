using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UnitType
{
    Empty,
    Grass,
    Factor,
    Defend,
    House,
}


public class TerrainUnit : MonoBehaviour
{

    public Vector3 position;
    private GameObject pieceInstance;
    public GameObject piecePrefab;

    public GameObject pieceParent;
    public UnitType type;
    public bool SetType(UnitType t)
    {
        if (this.type != UnitType.Empty || pieceInstance != null)
        {
            return false;
        }

        Debug.Log("Placed");
        this.type = t;
        this.pieceInstance = Instantiate(piecePrefab, transform.position+new Vector3(0,0.5f,0), Quaternion.identity);
        this.pieceInstance.transform.SetParent(this.pieceParent.transform);
        return true;
    }
    void Start()
    {
        type = UnitType.Empty;
    }

}
