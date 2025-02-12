﻿using System.Runtime.CompilerServices;
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
    Storage = 104,

    Arrow = 105, // tower 1
    Absorb = 106, // tower 2

    Spawn = 110,


    // Generated
    Mine = 200,
    Tower = 201,
    Portal = 202,

    // Static
    Void = 300,// 虚空，不渲染该方块
    Rock = 301
}

public enum UnitBuffType
{
    NONE = 0,
    INCREASE_FACTORY = 100,
    INCREASE_GRASS = 101
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
    /// 是否为空方块（供连续判断）
    /// </summary>
    public static bool IsEmptyType(UnitType t)
    {
        int i = (int)t;
        if (i >= 100 && i < 300)
            return false;
        return true;
    }
    /// <summary>
    /// 是否为生产方块（供渲染进度条）
    /// </summary>
    public static bool IsProduceType(UnitType t)
    {
        return t == UnitType.Grass || t == UnitType.Factor;
    }
    public static bool IsStaticType(UnitType t)
    {
        return ((int)t >= 300 && (int)t < 400);
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
            case UnitType.Storage://195, 77, 134
                return new Color(195 / 255.0f, 77 / 255.0f, 134 / 255.0f);
            case UnitType.Absorb://63, 180, 171
                return new Color(63 / 255.0f, 180 / 255.0f, 171 / 255.0f);
            case UnitType.Spawn://101, 103, 101
                return new Color(101 / 255.0f, 103 / 255.0f, 101 / 255.0f);

            case UnitType.Rock://67, 67, 67
                return new Color(67 / 255.0f, 67 / 255.0f, 67 / 255.0f);

            case UnitType.Tower://199, 62, 58
                return new Color(199 / 255.0f, 62 / 255.0f, 58 / 255.0f);

            case UnitType.Mine:// KIKYO 986D9C
                return new Color(0x98 / 255.0f, 0x6D / 255.0f, 0x9c / 255.0f);
            default:
                return new Color(67 / 255.0f, 67 / 255.0f, 67 / 255.0f);
        }
    }
    public static Color GetColorByBuffType(UnitBuffType t)
    {
        switch (t)
        {
            case UnitBuffType.INCREASE_FACTORY:
                return GetColorByType(UnitType.Mine);
            case UnitBuffType.INCREASE_GRASS:
                return GetColorByType(UnitType.Grass);
            default:
                return Color.white;
        }
    }
    public Vector3Int position;
    private GameObject pieceInstance;
    public GameObject piecePrefab;

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
    public GameObject buffPrefab;

    /// (runtime)若有额外元件则添加至此
    private GameObject subInstance;

    private PlayManager playManager;
    [HideInInspector]
    public WorldManager worldManager;
    private TerrainUnitConfig unitConfig;
    public bool SetType(UnitType t)
    {
        if (this.type != UnitType.Empty && pieceInstance != null || t == UnitType.Empty)
        {
            ClearOut();
            Destroy(pieceInstance);
            if (subInstance != null)
                Destroy(subInstance);
        }

        if (!InitStaticType(t))
        {


            this.type = t;
            if (t == UnitType.Empty)
            {
                // reset
                _canProducing = false;
                isProducer = false;
                return false;
            }

            this.pieceInstance = Instantiate(piecePrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            this.pieceInstance.transform.SetParent(this.transform);
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
                this.subPrefab = unitConfig.GetProducePrefab(this.type);// for static prefabs
                if (subPrefab != null)
                {
                    this.subInstance = Instantiate(subPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                }
            }
            // default Prefab
            switch (t)
            {
                case UnitType.Mine:
                    subPrefab = MinePrefab;
                    // static Instance
                    subInstance = Instantiate(MinePrefab, transform.position + new Vector3(-0.5f, 0.5f, -0.5f), Quaternion.identity);
                    //  Random Generate Factor Range
                    var buffRange = worldManager.SpreadBFS(this.position,
                    (me, him) => (me.position - him.position).magnitude < 2.3f && Math.Abs(me.position.y - him.position.y) <= 1);
                    foreach (var item in buffRange)
                    {
                        worldManager.GetUnit(item).localBuff = UnitBuffType.INCREASE_FACTORY;
                    }
                    // flush buff mesh
                    break;
                case UnitType.Storage:
                    playManager.IncreaseMaxResource();//  every storage take 1
                    break;
                default:
                    break;
            }
        }
        return true;

    }
    /// <summary>
    /// Do Dead Stuff
    /// </summary>
    private void ClearOut()
    {
        switch (type)
        {
            case UnitType.Storage:
                playManager.DecreaseMaxResource();
                break;
            default: break;
        }
    }

    public void SetEmpty()
    {
        if (IsManualType(type))
        {
            SetType(UnitType.Empty);
        }
    }

    /// <summary>
    /// Special Terrain
    /// </summary>
    /// <return>是否要截断setType</return>
    bool InitStaticType(UnitType _type)
    {
        // already generated => skip
        if ((int)type >= 200 && (int)type < 400) return false;//continue
        if ((int)_type >= 300 && (int)_type < 400)
        {
            // in static case, dont replace type in order keep beautified
            if (this.type != UnitType.Empty) return false;//continue
            this.type = _type;

            switch (_type)
            {
                case UnitType.Void:
                    this.GetComponent<MeshFilter>().mesh = null;
                    this.GetComponent<Collider>().enabled = false;
                    break;
                case UnitType.Rock:
                    return false;
                default: break;
            }
            return true;
        }
        return false;
    }

    public void SetBuff(UnitBuffType buffType)
    {
        if (localBuff == UnitBuffType.NONE)
            this.localBuff = buffType;

    }

    void Awake()
    {
        // if(!type) type = UnitType.Empty;
        this.playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        this.unitConfig = GameObject.Find("UnitConfig").GetComponent<TerrainUnitConfig>();
    }

    private void Update()
    {
        if (progressFiller && isProducer && canProducing)
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
                subInstance = Instantiate(subPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                subInstance.transform.SetParent(transform);
                DropResource dr = subInstance.GetComponent<DropResource>();
                if (dr != null)
                {
                    dr.position = this.position;
                }
                localProgress -= localPeriod;
                canProducing = false;
            }
            else
            {
                localProgress += Time.fixedDeltaTime;
                BuffConsult();
            }
        }
        BuffConsult();
    }
    /// <summary>
    /// Buff影响
    /// </summary>
    private void BuffConsult()
    {
        if (isProducer && canProducing)
        {
            switch (type)
            {
                case UnitType.Grass:
                    // 越高速度越快
                    localProgress += Time.fixedDeltaTime * this.position.y / worldManager.GetMaxHight();
                    if (localBuff == UnitBuffType.INCREASE_GRASS)
                        localProgress += Time.fixedDeltaTime;
                    break;
                case UnitType.Factor:
                    if (localBuff == UnitBuffType.INCREASE_FACTORY)
                        localProgress += 2 * Time.fixedDeltaTime;
                    break;
                default: break;
            }
        }
    }

    public void OnLeaveMap()
    {
        GetComponent<DropFall>().Fall();

        this.SetType(UnitType.Empty);
        if (subInstance) Destroy(subInstance);
        if (pieceInstance) Destroy(pieceInstance);

        gameObject.SetActive(false);
        // StartCoroutine("FallinDown")    :P
    }


    public void OnEnterMap(Vector3Int position)
    {
        gameObject.SetActive(true);
        this.position = position;
        this.transform.position = GameUtils.PositionToTranform(position);
    }


    private void OnMouseEnter()
    {
        playManager.UpdateSlectingPosition(this);
    }
}
