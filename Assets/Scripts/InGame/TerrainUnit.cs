using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainUnit : MonoBehaviour
{

    public Vector3 position;
    public GameObject instance;
    
    
    public TerrainUnit(Vector3 position, GameObject instance){
        this.position = position;
        this.instance = instance;
    }
}
