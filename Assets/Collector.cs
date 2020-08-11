using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{

    WorldManager worldManager;
    // Start is called before the first frame update
    void Start()
    {
        worldManager = GameObject.Find("Map").GetComponent<WorldManager>();
    }
    private int c = 5;
    private int _c = 0;
    private UnitType standingType;
    // Update is called once per frame
    void Update()
    {

        // cut by clock differ
        if (_c++ < c) return;
        _c = 0;


        standingType = worldManager.GetUnit(transform.position).type;
        if (standingType == UnitType.Absorb)
        {
            transform.localScale = Vector3.one * 10;
        }
        else
        {
            transform.localScale = Vector3.one;
        }

    }
}
