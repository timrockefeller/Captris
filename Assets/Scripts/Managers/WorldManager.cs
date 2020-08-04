using System.Drawing;
using System;
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
    public static Vector2Int _size { get; private set; }
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
    private float _relief = 14f;
    private int _maxHeight = 5;
    private int poolCur = 0;
    private BuffEffectManager buffEffectManager;
    // Start is called before the first frame update
    void Awake()
    {
        _size = size;

        map = new TerrainUnit[this.size.x, this.size.y];

        RD.SetSeed(iSeed);
        buffEffectManager = GameObject.Find("BuffEffectManager").GetComponent<BuffEffectManager>();
        this.Generate();

        StartCoroutine("LongtimeForward");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) Forward();
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
        int[,] heightMap = new int[size.x, size.y];
        ///////////// generate world
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {

                float xSample = (x + _seedX) / _relief;
                float zSample = (z + _seedZ) / _relief;
                float noise = Mathf.PerlinNoise(xSample, zSample) * 1.2f - 0.2f;
                noise = Mathf.Pow(noise, 2);


                heightMap[x, z] = (int)Mathf.Clamp(Mathf.Floor(_maxHeight * noise), 0, 1000);
                // int y = 0;

            }
        }
        // 整流一波
        bool changed = true;
        int maxChange = 5;
        while (maxChange-- > 0 && changed)
        {
            changed = false;
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.y; z++)
                {
                    int tmp = heightMap[x, z], tc = heightMap[x, z], ts = 1;
                    foreach (var item in GetNeiboursVector(x, z))
                    {
                        tc += heightMap[item.x, item.y];
                        ts++;
                    }
                    int tr = (int)Mathf.Round(tc * 1.0F / ts);
                    if (tmp != tr)
                    {
                        changed = true;
                        heightMap[x, z] = tr;
                    }
                }
            }
        }
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                GameObject ground = Instantiate(pGround, GameUtils.PositionToTranform(new Vector3Int(x, heightMap[x, z], z)), Quaternion.identity);
                ground.transform.SetParent(transform);
                this.map[x, z] = ground.GetComponent<TerrainUnit>();
                this.map[x, z].position = new Vector3Int(x, heightMap[x, z], z);
                this.map[x, z].worldManager = this;
            }
        }



        ///////////// Generate Static Terrains
        /// 
        /////// Towers
        int twOutBorder = 7, twInnerBorder = 15;
        Vector2Int towerpos1 = RD.NextPosition(size.x / 2 - twOutBorder - twInnerBorder, size.y / 2 - twOutBorder - twInnerBorder) + new Vector2Int(twOutBorder, twInnerBorder + size.y / 2);
        Vector2Int towerpos2 = RD.NextPosition(size.x / 2 - twOutBorder - twInnerBorder, size.y / 2 - twOutBorder - twInnerBorder) + new Vector2Int(twInnerBorder + size.x / 2, twOutBorder);
        UnitType[,] tu1 = StaticTerrain.NextTower();
        for (int _i = 0; _i < tu1.GetLength(0); _i++)
        {
            for (int _j = 0; _j < tu1.GetLength(1); _j++)
            {
                GetUnit(_i - tu1.GetLength(0) / 2 + towerpos1.x, _j - tu1.GetLength(1) / 2 + towerpos1.y).SetType(tu1[_i, _j]);
            }
        }
        UnitType[,] tu2 = StaticTerrain.NextTower();
        for (int _i = 0; _i < tu2.GetLength(0); _i++)
        {
            for (int _j = 0; _j < tu2.GetLength(1); _j++)
            {
                GetUnit(_i - tu2.GetLength(0) / 2 + towerpos2.x, _j - tu2.GetLength(1) / 2 + towerpos2.y).SetType(tu2[_i, _j]);
            }
        }
        /////// Rocks
        int collapseNum = (int)(RD.NextDouble() * 0 + 1);
        // 1. collapse
        while (collapseNum-- > 0)
        {

        }
        PoissonDiscSampler sampler;
        int sampleOffset;
        /////// Voids
        // sampleOffset = 5;// 裁去边框
        // sampler = new PoissonDiscSampler(size.x - 2 * sampleOffset, size.y - 2 * sampleOffset, 5f);// 在少2倍边框区域内随机生成点
        // foreach (Vector2 sample in sampler.Samples())
        // {
        //     var dir = (int)(RD.NextDouble() * 4);
        //     Vector3Int dirVector;
        //     switch (dir)
        //     {
        //         case 0:
        //             dirVector = Vector3Int.right; break;
        //         case 1:
        //             dirVector = Vector3Int.left; break;
        //         case 2:
        //             dirVector = new Vector3Int(0, 0, -1); break;
        //         case 3:
        //         default:
        //             dirVector = new Vector3Int(0, 0, 1); break;
        //     }
        //     int i = 0, maxInter = 100;
        //     bool camPlace = true;
        //     while (maxInter-- > 0 && GetUnit((int)sample.x + i * dirVector.x + sampleOffset,
        //                    (int)sample.y + i * dirVector.z + sampleOffset) != null)
        //     {
        //         if (GetUnit((int)sample.x + i * dirVector.x + sampleOffset,
        //                     (int)sample.y + i * dirVector.z + sampleOffset).type != UnitType.Empty)
        //         {
        //             camPlace = false;
        //             break;
        //         }
        //         i++;
        //     }
        //     if (camPlace)
        //     {
        //         i = 1; maxInter = 100;
        //         while (maxInter-- > 0 && GetUnit((int)sample.x + i * dirVector.x + sampleOffset,
        //                   (int)sample.y + i * dirVector.z + sampleOffset) != null)
        //         {
        //             GetUnit((int)sample.x + i * dirVector.x + sampleOffset,
        //                         (int)sample.y + i * dirVector.z + sampleOffset).SetType(UnitType.Void);

        //             i++;
        //         }
        //     }
        // }

        ///////////// TEMPLATE bfs
        // foreach (var item in SpreadBFS(map[15, 15].position,
        //     (a, b) => ((a.position - b.position).magnitude < 2)
        // ))
        // {
        //     GetUnit(item).SetType(UnitType.Spawn);
        // }
        /////////////
        ///  BuffEffect be = GameObject.Find("TestBuff").GetComponent<BuffEffect>();
        // //buff test
        // be.AttachMesh(BuffEffectManager.GetLineByVectors(SpreadBFS(map[15, 15].position,
        //     (a, b) => ((a.position - b.position).magnitude < 2)
        // )));


        ///////////// generate Mines
        //  http://www.twinklingstar.cn/2013/406/stochastic-distributed-ray-tracing/
        //  Poisson Disk Distribution
        sampleOffset = 2;// 裁去边框
        sampler = new PoissonDiscSampler(size.x - sampleOffset * 2, size.y - sampleOffset * 2, 15f);
        foreach (Vector2 sample in sampler.Samples())
        {
            // // Instantiate(pGround, new Vector3(sample.x,10,sample.y),Quaternion.identity);
            Vector2Int centerPos = new Vector2Int((int)sample.x + sampleOffset, (int)sample.y + sampleOffset);
            UnitType[,] mineM = StaticTerrain.NextModule();
            for (int _i = 0; _i < mineM.GetLength(0); _i++)
            {
                for (int _j = 0; _j < mineM.GetLength(1); _j++)
                {
                    if (mineM[_i, _j] != UnitType.Empty)
                        GetUnit(_i - mineM.GetLength(0) / 2 + centerPos.x, _j - mineM.GetLength(1) / 2 + centerPos.y).SetType(mineM[_i, _j]);
                }
            }

        }

        ///////////// generate spawn point & player
        map[size.x / 2, size.y / 2].SetType(UnitType.Spawn);
        playerInstance = Instantiate(playerPrefab, new Vector3(size.x / 2 + 0.5f, 15, (size.y / 2) + 0.5f), Quaternion.identity);



    }
    private void Start()
    {
        ///////////// Fresh Buff Area
        buffEffectManager.RefreshBuff();
    }

    /// <summary>
    /// 查找四周有无符合条件的邻居
    /// 
    /// t: 留空则搜索 IsManualType
    /// </summary>
    public bool HasNeibour(int x, int y, UnitType t = UnitType.Empty)
    {
        // x += poolCur;
        x %= size.x;
        for (int i = 0; i < 4; i++)
        {
            if (_4direction[i, 0] + x < 0 || _4direction[i, 0] + x >= size.x
             || _4direction[i, 1] + y < 0 || _4direction[i, 1] + y >= size.y) continue;
            if (t == UnitType.Empty)
            {
                if (!TerrainUnit.IsManualType(map[_4direction[i, 0] + x, _4direction[i, 1] + y].type))// type
                    continue;

            }
            else
            {
                if (map[_4direction[i, 0] + x, _4direction[i, 1] + y].type != t)// type
                    continue;
            }

            if (1 >= Mathf.Abs(map[_4direction[i, 0] + x, _4direction[i, 1] + y].position.y - map[x, y].position.y))// 0<= dy <= 1
                return true;
        }
        return false;
    }
    IEnumerable<TerrainUnit> GetNeibours(int x, int y)
    {
        x %= size.x;
        for (int i = 0; i < 4; i++)
        {
            if (((_4direction[i, 0] + x) < 0)
             || ((_4direction[i, 0] + x) >= size.x)
             || (_4direction[i, 1] + y < 0)
             || (_4direction[i, 1] + y >= size.y)
             )
                continue;
            TerrainUnit t = null;
            try
            {
                t = map[_4direction[i, 0] + x, _4direction[i, 1] + y];


            }
            catch (System.Exception)
            {

                Debug.Log((_4direction[i, 0] + x) + " " + (_4direction[i, 1] + y));
            }
            if (t != null) yield return t;
        }
    }
    IEnumerable<Vector2Int> GetNeiboursVector(int x, int y)
    {
        x %= size.x;
        for (int i = 0; i < 4; i++)
        {
            if (((_4direction[i, 0] + x) < 0)
             || ((_4direction[i, 0] + x) >= size.x)
             || (_4direction[i, 1] + y < 0)
             || (_4direction[i, 1] + y >= size.y)
             )
                continue;
            yield return new Vector2Int(_4direction[i, 0] + x, _4direction[i, 1] + y);
        }
    }

    public int GetMaxHight()
    {
        return _maxHeight;
    }

    public TerrainUnit GetUnit(int x, int y)
    {
        // x += poolCur;
        x %= size.x;
        if (x < 0 || x >= size.x
            || y < 0 || y >= size.y) return null;
        return map[x, y];
    }
    public TerrainUnit GetUnit(Vector3Int v)
    {
        return GetUnit(v.x, v.z);
    }

    public List<Vector3> GetRoundSameType(int x, int y)
    {

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

                // Move to new position
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

    /// <summary>
    /// 根据条件函数深度优先搜索符合的位置
    /// </summary>
    /// <param name="v">初始点</param>
    /// <param name="condition">条件</param>
    /// <param name="pass">中间件</param>
    /// <returns></returns>
    public List<Vector3Int> SpreadBFS(Vector3Int v, Func<TerrainUnit, TerrainUnit, bool> condition, Func<Vector3Int, bool> callback = null, List<Vector3Int> pass = null)
    {
        if (pass == null) pass = new List<Vector3Int>();
        var waitingQueue = new Queue<Vector3Int>();
        var visited = new bool[size.x + 2, size.y + 2];
        waitingQueue.Enqueue(v);
        while (waitingQueue.Count > 0)
        {
            Vector3Int t = waitingQueue.Dequeue();
            visited[t.x + 1, t.z + 1] = true;
            if (condition(GetUnit(v.x, v.z), GetUnit(t.x, t.z)))
            {
                pass.Add(t);
                if (callback != null) callback(t);
                foreach (TerrainUnit i in GetNeibours(t.x, t.z))
                {
                    if (!visited[i.position.x + 1, i.position.z + 1])
                        waitingQueue.Enqueue(i.position);
                }
            }
        }
        return pass;
    }
}
