using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


public class TerrainUnit : MonoBehaviour
{
    /// <summary>
    /// 是否玩家放置方块（供连续判断）
    /// </summary>
    public static bool IsManualType(UnitType t)
    {
        int i = (int)t;
        // if(i !=0 ){
        // Debug.Log(i);
        // }
        if (i >= 100 && i < 200)
            return true;
        return false;
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

    [Header("References")]
    public GameObject MinePrefab;


    /// <summary>
    /// 若有额外元件则添加至此
    /// </summary>
    private GameObject subInstance;

    private PlayManager playManager;
    public bool SetType(UnitType t)
    {
        if (this.type != UnitType.Empty && pieceInstance != null || t == UnitType.Empty)
        {
            Destroy(pieceInstance);
        }
        // Debug.Log("Placed");
        this.type = t;
        this.pieceInstance = Instantiate(piecePrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        this.pieceInstance.transform.SetParent(this.pieceParent.transform);
        this.pieceInstance.GetComponent<MeshRenderer>().material.SetColor("_Color", TerrainUnit.GetColorByType(this.type));

        // default placements
        switch (t)
        {
            case UnitType.Mine:
                subInstance = Instantiate(MinePrefab, transform.position + new Vector3(-0.5f, 0.5f, -0.5f), Quaternion.identity);
                break;
            default:
                break;
        }


        return true;

    }
    void Start()
    {
        // if(!type) type = UnitType.Empty;
        this.playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
    }

    void LateUpdate()
    {


    }



    private void OnMouseEnter()
    {
        playManager.UpdateSlectingPosition(this);
    }
}
