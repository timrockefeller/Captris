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
    public Vector2Int size = new Vector2Int(10, 20);

    [Header("Generate Prefabs")]
    public GameObject playerPrefab;
    public GameObject playerInstance;
    private float _seedX;
    private float _seedZ;


    [Header("Forward Progress")]
    public float forwardSpeed = 10;
    public int forwardStep = 1;
    public GameObject poolPosInstane;


    // [SerializeField]
    private float _relief = 15f;
    private int _maxHeight = 5;
    private int poolCur = 0;

    // Start is called before the first frame update
    void Awake()
    {
        map = new TerrainUnit[this.size.x, this.size.y];

        RD.SetSeed(iSeed);

        this.Generate();

        StartCoroutine("LongtimeForward");
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.N)) Forward();
    }
    IEnumerator LongtimeForward()
    {
        while (true)
        {
            yield return new WaitForSeconds(forwardSpeed);
            Forward(forwardStep);
            poolPosInstane.GetComponent<Mover>().MoveTo(new Vector3(poolCur, 0, 0));
        }
    }

    public void Generate()
    {
        poolCur = 0;
        _seedX = (float)(RD.NextDouble() * 100.0);
        _seedZ = (float)(RD.NextDouble() * 100.0);

        // generate world
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
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
        PoissonDiscSampler sampler = new PoissonDiscSampler(size.x, size.y, 15f);
        foreach (Vector2 sample in sampler.Samples())
        {
            // Instantiate(pGround, new Vector3(sample.x,10,sample.y),Quaternion.identity);
            map[(int)sample.x, (int)sample.y].SetType(UnitType.Mine);
        }

        map[10, 2].SetType(UnitType.Spawn);
        // generate spawn point & player
        map[size.x / 2, size.y / 2].SetType(UnitType.Spawn);
        playerInstance = Instantiate(playerPrefab, new Vector3(size.x / 2 + 0.5f, 15, (size.y / 2) + 0.5f), Quaternion.identity);
        // Forward(6);
    }

    public bool HasNeibour(int x, int y)
    {
        // x += poolCur;
        x %= size.x;
        for (int i = 0; i < _4direction.Length / 2; i++)
        {
            if ((_4direction[i, 0] + x) % size.x < 0 || (_4direction[i, 0] + x) % size.x >= size.x
             || _4direction[i, 1] + y < 0 || _4direction[i, 1] + y >= size.y) continue;
            if (TerrainUnit.IsManualType(map[_4direction[i, 0] + x, _4direction[i, 1] + y].type))
                return true;
        }
        return false;
    }

    public TerrainUnit GetUnit(int x, int y)
    {
        // x += poolCur;
        x %= size.x;
        if (x < 0 || x >= size.x
            || y < 0 || y >= size.y) return null;
        return map[x, y];
    }


    public List<Vector3> GetRoundSameType(int x, int y)
    {
        // TODO
        var rst = new List<Vector3>();
        var queue = new Queue<Vector2>();
        bool[,] visited = new bool[size.x, size.y];
        return rst;
    }

    public void Forward(int step = 1)
    {
        int c = step;
        while (c-- > 0)
        {
            for (int z = 0; z < size.y; z++)
            {

                // TODO Move to new position
                map[poolCur % size.x, z % size.y].OnLeaveMap();
                // continurous perlin noise ganeration
                float xSample = (poolCur + size.x + _seedX) / _relief;
                float zSample = (z + _seedZ) / _relief;
                float noise = Mathf.PerlinNoise(xSample, zSample);
                int y = (int)Mathf.Floor(_maxHeight * noise);
                map[poolCur % size.x, z % size.y].OnEnterMap(new Vector3Int(poolCur + size.x, y, z));
            }
        }
        this.poolCur += step;
    }

}
