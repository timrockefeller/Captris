using System;
using UnityEngine;

public class TerrainUnitConfig : MonoBehaviour
{
    [Serializable]
    public struct UnitConfig
    {
        public UnitType type;
        public bool isProducer;
        public float producePeriod;
        public GameObject producePrefab;
        public int[] resourceNeeded;
    }
    public UnitConfig[] unitConfig;
    [Serializable]
    public struct ResourceConfig
    {
        public ResourceType type;
        public int initialNumber;
    }
    public ResourceConfig[] resourceConfig;
    public ResourceConfig GetConfig(ResourceType t)
    {
        int pc = resourceConfig.Length;
        while (pc-- > 0)
        {
            if (resourceConfig[curRes].type == t) return resourceConfig[curRes];
            curRes = (curRes + 1) % resourceConfig.Length;
        }
        return resourceConfig[0];
    }
    public UnitConfig GetConfig(UnitType t)
    {
        int pc = unitConfig.Length;
        while (pc-- > 0)
        {
            if (unitConfig[curUnit].type == t) return unitConfig[curUnit];
            curUnit = (curUnit + 1) % unitConfig.Length;
        }
        return unitConfig[0];
    }
    public bool IsProducer(UnitType t)
    {
        return GetConfig(t).isProducer;
    }
    public float GetProducePeriod(UnitType t)
    {
        return GetConfig(t).producePeriod;
    }
    public GameObject GetProducePrefab(UnitType t)
    {
        return GetConfig(t).producePrefab;
    }

    /// <summary>
    /// accelerate same type reading
    /// </summary>
    private int curUnit = 0;
    private int curRes = 0;
}