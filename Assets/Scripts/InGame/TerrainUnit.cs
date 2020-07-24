using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitType
{
    Empty = 0,
    // Nanual
    Road = 100,
    Grass = 101,
    Factor = 102,
    Defend = 103,
    Spawn = 110,
    // Generated
    Mine = 201,
    Tower = 202,
    Portal = 203
}

public enum UnitBuffType
{
    NONE = 0,
    INCREASE_FACTORY = 100,

}

public class TerrainUnit : MonoBehaviour
{


    /// <summary>
    /// 是否玩家放置方块（供连续判断）
    /// </summary>
    public static bool IsManualType(UnitType t)
    {
        int i = (int)t;
        if (i >= 100 && i < 200)
            return true;
        return false;
    }

    /// <summary>
    /// 是否为生产方块（供渲染进度条）
    /// </summary>
    public static bool IsProduceType(UnitType t)
    {
        return t == UnitType.Grass || t == UnitType.Factor;
    }

    public static Color GetColorByType(UnitType t)
    {
        switch (t)
        {
            case UnitType.Empty:
                return Color.red;

            case UnitType.Road://130, 100, 89
                return new Color(130 / 255.0f, 100 / 255.0f, 89 / 255.0f);
            case UnitType.Grass://124, 168, 44
                return new Color(124 / 255.0f, 168 / 255.0f, 44 / 255.0f);
            case UnitType.Factor://222, 162, 30
                return new Color(222 / 255.0f, 162 / 255.0f, 30 / 255.0f);
            case UnitType.Defend://44, 118, 179
                return new Color(44 / 255.0f, 118 / 255.0f, 179 / 255.0f);
            case UnitType.Spawn://101, 103, 101
                return new Color(101 / 255.0f, 103 / 255.0f, 101 / 255.0f);


            case UnitType.Mine:// KIKYO 986D9C
                return new Color(0x98 / 255.0f, 0x6D / 255.0f, 0x9c / 255.0f);
            default:
                return Color.white;
        }
    }
    public Vector3Int position;
    private GameObject pieceInstance;
    public GameObject piecePrefab;

    public GameObject pieceParent;
    public UnitType type;


    //////////////////////
    /// production module
    [Header("Ingame Production")]
    private bool isProducer = false;
    public float localProgress;// current time
    public float localPeriod;// total time
    public UnitBuffType localBuff = UnitBuffType.NONE;
    private Image progressFiller;
    private Image progressBackground;

    private bool _canProducing = false;
    public bool canProducing
    {
        get
        {
            return _canProducing;
        }
        set
        {
            if (isProducer)
            {
                if (progressFiller) progressFiller.fillAmount = value ? 1 : 0;
                if (progressBackground) progressBackground.fillAmount = value ? 1 : 0;
                _canProducing = value;
            }
        }
    }


    //////////////////////
    /// Use for prefabs
    [Header("References Prefabs")]
    public GameObject MinePrefab;
    private GameObject subPrefab;

    /// (runtime)若有额外元件则添加至此
    private GameObject subInstance;

    private PlayManager playManager;
    private TerrainUnitConfig unitConfig;
    public bool SetType(UnitType t)
    {
        if (this.type != UnitType.Empty && pieceInstance != null || t == UnitType.Empty)
        {
            Destroy(pieceInstance);
            if (subInstance != null)
                Destroy(subInstance);
        }
        if (t == UnitType.Empty) return false;

        this.type = t;
        this.pieceInstance = Instantiate(piecePrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        this.pieceInstance.transform.SetParent(this.pieceParent.transform);
        this.pieceInstance.GetComponent<MeshRenderer>().material.SetColor("_Color", TerrainUnit.GetColorByType(this.type));


        this.progressFiller = this.pieceInstance.transform.Find("Canvas").Find("Filler").GetComponent<Image>();
        this.progressBackground = this.pieceInstance.transform.Find("Canvas").Find("Back").GetComponent<Image>();
        // produce handle
        this.isProducer = unitConfig.IsProducer(this.type);
        if (this.isProducer)
        {
            this.localPeriod = unitConfig.GetProducePeriod(this.type);
            this.subPrefab = unitConfig.GetProducePrefab(this.type);
            this.canProducing = true;
        }
        else
        {
            this.canProducing = false;
        }
        // default Prefab
        switch (t)
        {
            case UnitType.Mine:
                subPrefab = MinePrefab;
                // static Instance
                subInstance = Instantiate(MinePrefab, transform.position + new Vector3(-0.5f, 0.5f, -0.5f), Quaternion.identity);
                // TODO: Random Generate Factor Range
                localBuff = UnitBuffType.INCREASE_FACTORY;// spread it!
                break;

            default:
                break;
        }
        return true;

    }
    void Awake()
    {
        // if(!type) type = UnitType.Empty;
        this.playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        this.unitConfig = GameObject.Find("UnitConfig").GetComponent<TerrainUnitConfig>();
    }

    private void Update()
    {
        if (isProducer && canProducing)
        {
            progressFiller.fillAmount = localProgress / localPeriod;

        }
    }

    private void FixedUpdate()
    {
        if (isProducer && canProducing)
        {
            if (localProgress > localPeriod)
            {
                // Instantiate produce prefab
                Instantiate(subPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                localProgress -= localPeriod;
                canProducing = false;
            }
            else
            {
                localProgress += Time.fixedDeltaTime;
            }
        }
    }


    private void OnMouseEnter()
    {
        playManager.UpdateSlectingPosition(this);
    }
}
