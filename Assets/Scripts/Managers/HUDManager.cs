using System;
using UnityEngine;

public class HUDManager : MonoBehaviour
{

    /// <summary>
    /// 放置CD
    /// </summary>

    [Serializable]
    public struct S_UnitType
    {
        public UnitType type;
        public bool enabled;
        public float cooldownTotal;
        [HideInInspector]
        public float cooldownCurrent;

        public GameObject cooldownFillarea;
        public GameObject cooldownButton;

    }
    public S_UnitType[] units;
    
    /// <summary>
    /// 是否正在冷却中
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool IsCoolingdown(UnitType t)
    {
        foreach (var s in units)
        {
            if (s.type == t) return s.cooldownCurrent > 0;
        }
        return false;
    }

    /// <summary>
    /// 修改可用状态
    /// </summary>
    /// <param name="t"></param>
    /// <param name="e"></param>
    /// <returns>是否有修改</returns>
    public bool SetEnable(UnitType t, bool e){
        bool rsl = false;
        for (var i =0;i<units.Length;i++)
        {
            if(units[i].type == t){
                rsl = units[i].enabled != e;
                units[i].enabled = e;
                // TODO show or hide HUD
            }
        }
        return rsl;
    } 
    private void FixedUpdate()
    {

    }

}
