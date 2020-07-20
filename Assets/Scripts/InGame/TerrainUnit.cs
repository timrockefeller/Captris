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
    public GameObject instance;
    
    public UnitType type;

    void Start()
    {
        type = UnitType.Empty;
    }
    public TerrainUnit(Vector3 position, GameObject instance){
        this.position = position;
        this.instance = instance;
    }
}
