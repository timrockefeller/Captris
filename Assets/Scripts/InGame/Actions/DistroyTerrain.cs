using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistroyTerrain : MonoBehaviour
{
    // private bool triggered = false;

    [Header("Explotions")]
    public GameObject explosionPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain")
        {
            // if (!triggered)
            //     other.GetComponent<TerrainUnit>().SetEmpty();

            GameObject ins = Instantiate(explosionPrefab,transform.position,Quaternion.identity);
            // ins.transform.SetParent(null);
            // triggered = true;
        }
    }

}
