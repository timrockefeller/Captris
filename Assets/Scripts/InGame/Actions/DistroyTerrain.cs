using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistroyTerrain : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain")
        {
            if (!triggered)
                other.GetComponent<TerrainUnit>().SetType(UnitType.Empty);
            triggered = true;
        }
    }

}
