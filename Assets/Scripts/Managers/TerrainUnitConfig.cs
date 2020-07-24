using System;
using UnityEngine;

public class TerrainUnitConfig : MonoBehaviour {
    [Serializable]
    public struct UnitConfig
    {
        public UnitType type;
        public bool isProducer;
        public float producePeriod;
    }
    public UnitConfig[] config;

    public UnitConfig GetConfig(UnitType t){
        for (int i = 0; i < config.Length; i++)
        {
            if(config[i].type == t)return config[i];
        }
        return config[0];
    }
    public bool IsProducer(UnitType t){
        return GetConfig(t).isProducer;
    }
    public float GetProducePeriod(UnitType t){
        return GetConfig(t).producePeriod;
    }
}