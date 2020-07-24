using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldManager : MonoBehaviour
{

    private static int[,] _4direction = new int[,] {
        { 1, 0 },
        { -1, 0 },
        { 0, -1 },
        { 0, 1 }
    };

    public GameObject pGround;
    public int iSeed;

    [HideInInspector]
    public TerrainUnit[,] map;
    public int size = 32;

    [Header("Generate Prefabs")]
    public GameObject playerPrefab;
    public GameObject playerInstance;
    private float _seedX;
    private float _seedZ;

    // [SerializeField]
    private float _relief = 15f;
    private int _maxHeight = 5;

    // Start is called before the first frame update
    void Awake()
    {
        map = new TerrainUnit[this.size, this.size];

        RD.SetSeed(iSeed);

        this.Generate();
    }

    public void Generate()
    {

        _seedX = (float)(RD.NextDouble() * 100.0);
        _seedZ = (float)(RD.NextDouble() * 100.0);

        // generate world
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
                this.map[x, z] = ground.GetComponent<TerrainUnit>();
                this.map[x, z].position = new Vector3Int(x, y, z);
            }
        }

        /// generate Mines
        //  http://www.twinklingstar.cn/2013/406/stochastic-distributed-ray-tracing/
        //  Poisson Disk Distribution
        PoissonDiscSampler sampler = new PoissonDiscSampler(size, size, 10f);
        foreach (Vector2 sample in sampler.Samples())
        {
            // Instantiate(pGround, new Vector3(sample.x,10,sample.y),Quaternion.identity);
            map[(int)sample.x, (int)sample.y].SetType(UnitType.Mine);
        }

        // generate spawn point & player
        map[15, 15].SetType(UnitType.Spawn);
        playerInstance = Instantiate(playerPrefab, new Vector3(15.5f, 15, 15.5f), Quaternion.identity);
    }

    public bool HasNeibour(int x, int y)
    {
        for (int i = 0; i < _4direction.Length / 2; i++)
        {
            if (_4direction[i, 0] + x < 0 || _4direction[i, 0] + x >= size
             || _4direction[i, 1] + y < 0 || _4direction[i, 1] + y >= size) continue;
            if (TerrainUnit.IsManualType(map[_4direction[i, 0] + x, _4direction[i, 1] + y].type))
                return true;
        }
        return false;
    }

    public TerrainUnit GetUnit(int x, int y)
    {
        if (x < 0 || x >= size
            || y < 0 || y >= size) return null;
        return map[x, y];
    }


    public List<Vector3> GetRoundSameType(int x, int y)
    {
        // TODO
        var rst =  new List<Vector3>();
        var queue = new Queue<Vector2>();
        bool [,]visited = new bool[size,size];
        return rst;
    }

}
