using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    public GameObject pGround;
    public int iSeed;

    [HideInInspector]
    public TerrainUnit[,] map;
    public int size = 32;
    private System.Random RD;


    private float _seedX;
    private float _seedZ;

    // [SerializeField]
    private float _relief = 15f;
    private int _maxHeight = 5;

    // Start is called before the first frame update
    void Awake()
    {
        map = new TerrainUnit[this.size, this.size];
        
        this.RD = new System.Random(iSeed);

        this.Generate();
    }

    public void Generate()
    {

        _seedX = (float)(this.RD.NextDouble() * 100.0);
        _seedZ = (float)(this.RD.NextDouble() * 100.0);

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float xSample = (x + _seedX) / _relief;
                float zSample = (z + _seedZ) / _relief;
                float noise = Mathf.PerlinNoise(xSample, zSample);

                int y = (int)Mathf.Floor(_maxHeight * noise);
                
                GameObject ground = Instantiate(pGround, new Vector3(x + 0.5f, y / 2.0f - 0.5f, z + 0.5f), Quaternion.identity);
                ground.transform.SetParent(transform);
                // this.map[x, z] = new TerrainUnit(new Vector3(x, y, z), ground);
            }
        }


    }

    public void Draw()
    {

    }
    public void Clear()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
