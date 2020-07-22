using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UnitType
{
    Empty,
    Grass,
    Factor,
    Defend,
    House,
}


public class TerrainUnit : MonoBehaviour
{
    public static Color GetColorByType(UnitType t)
    {
        switch (t)
        {
            case UnitType.Grass://124, 168, 44
                return new Color(124 / 255.0f, 168 / 255.0f, 44 / 255.0f);
            case UnitType.Factor://222, 162, 30
                return new Color(222 / 255.0f, 162 / 255.0f, 30 / 255.0f);
            case UnitType.Defend://44, 118, 179
                return new Color(44 / 255.0f, 118 / 255.0f, 179 / 255.0f);
            case UnitType.House://175, 86, 130
                return new Color(175 / 255.0f, 86 / 255.0f, 130 / 255.0f);
            default:
                return Color.red;
        }
    }
    public Vector3Int position;
    private GameObject pieceInstance;
    public GameObject piecePrefab;
    
    public GameObject pieceParent;
    public UnitType type;
    
    // references

    private PlayManager playManager;
    public bool SetType(UnitType t)
    {
        if (this.type != UnitType.Empty || pieceInstance != null)
        {
            return false;
        }
        // Debug.Log("Placed");
        this.type = t;
        this.pieceInstance = Instantiate(piecePrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        this.pieceInstance.transform.SetParent(this.pieceParent.transform);
        this.pieceInstance.GetComponent<MeshRenderer>().material.SetColor("_Color", TerrainUnit.GetColorByType(this.type));

        return true;

    }
    void Start()
    {
        type = UnitType.Empty;
        this.playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
    }

    private void OnMouseEnter() {
        playManager.UpdateSlectingPosition(this);
    }
}
