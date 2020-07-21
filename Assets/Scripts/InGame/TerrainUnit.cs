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
            case UnitType.Grass:
                return new Color(0x89 / 256.0f, 0xD1 / 256.0f, 0xA8 / 256.0f);
            case UnitType.Factor://F1B96F
                return new Color(0xF1 / 256.0f, 0xB9 / 256.0f, 0x6F / 256.0f);
            case UnitType.Defend://718DCF
                return new Color(0x71 / 256.0f, 0x8D / 256.0f, 0xCF / 256.0f);
            case UnitType.House://CB92BC
                return new Color(0xCB / 256.0f, 0x92 / 256.0f, 0xBC / 256.0f);
            default:
                return Color.black;
        }
    }
    public Vector3 position;
    private GameObject pieceInstance;
    public GameObject piecePrefab;

    public GameObject pieceParent;
    public UnitType type;
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
    }

}
