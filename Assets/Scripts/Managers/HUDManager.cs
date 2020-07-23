using System;
using UnityEngine;
using UnityEngine.UI;
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

        [Header("Cooldown Porperties")]
        public float cooldownTotal;
        public GameObject cooldownFillarea;
        public GameObject cooldownButton;


        /// Runtime

        [HideInInspector]
        public float cooldownCurrent;

        [HideInInspector]
        public Image cooldownFillareaCMP;

        [HideInInspector]
        public Button cooldownButtonCMP;
    }

    public S_UnitType[] units;


    /// <summary>
    /// 是否正在冷却中
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool IsCoolingdown(UnitType t)
    {
        for (var i = 0; i < units.Length; i++)
        {
            if (units[i].type == t) return IsCoolingdown(i);
        }
        return false;
    }
    private bool IsCoolingdown(int num)
    {
        return units[num].cooldownCurrent > 0;

    }

    /// <summary>
    /// 修改可用状态
    /// </summary>
    /// <param name="t"></param>
    /// <param name="e"></param>
    /// <returns>是否有修改</returns>
    public bool SetEnable(UnitType t, bool e)
    {

        for (var i = 0; i < units.Length; i++)
        {
            if (units[i].type == t)
            {
                return SetEnable(i, e);
            }
        }
        return false;
    }
    private bool SetEnable(int idx, bool e)
    {
        bool rsl = units[idx].enabled != e;
        units[idx].enabled = e;
        // TODO show or hide HUD
        if (rsl)
        {
            if (units[idx].cooldownButtonCMP != null)
            {
                units[idx].cooldownButtonCMP.interactable = e;
            }
        }
        return rsl;
    }

    private void Start()
    {
        for (var i = 0; i < units.Length; i++)
        {
            units[i].cooldownButtonCMP = units[i].cooldownButton.GetComponent<Button>();
            units[i].cooldownFillareaCMP = units[i].cooldownFillarea.GetComponent<Image>();
            units[i].cooldownFillarea.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        for (var i = 0; i < units.Length; i++)
        {
            if (IsCoolingdown(i))
            {
                units[i].cooldownCurrent -= Time.fixedDeltaTime;
                units[i].cooldownFillareaCMP.fillAmount = units[i].cooldownCurrent / units[i].cooldownTotal;
                if(!IsCoolingdown(i)){
                    units[i].cooldownFillarea.SetActive(false);
                }
            }
        }
    }
    public void Placed(UnitType t)
    {

        for (var i = 0; i < units.Length; i++)
        {
            if (units[i].type == t)
            {
                units[i].cooldownFillarea.SetActive(true);
                units[i].cooldownCurrent = units[i].cooldownTotal;
                return;
            }
        }
    }

}
