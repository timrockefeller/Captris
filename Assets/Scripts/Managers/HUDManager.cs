using System;
using System.Collections;
using System.Collections.Generic;
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
    [Header("")]
    public S_UnitType[] units;

    /// <summary>
    /// 资源显示
    /// </summary>

    [Serializable]
    public struct S_ResourceType
    {
        public ResourceType type;
        public GameObject text;
        [HideInInspector]
        public Text textCMP;
    }
    [Header("资源显示")]
    public S_ResourceType[] resources;
    private Dictionary<ResourceType, int> _res;
    private TerrainUnitConfig unitConfig;

    [Header("时间轴")]
    public GameObject timeBoard;
    private Image timeBoardCMP;


    [Header("Hint Box")]
    public GameObject hintBox;
    private Image hintBoxCMP;
    public bool showHintBox = false;

    private void Awake()
    {
        unitConfig = GameObject.Find("UnitConfig").GetComponent<TerrainUnitConfig>();
    }

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
        for (var i = 0; i < resources.Length; i++)
        {
            resources[i].textCMP = resources[i].text.GetComponent<Text>();
        }
        timeBoardCMP = timeBoard.GetComponent<Image>();
        hintBoxCMP = hintBox.GetComponent<Image>();
    }
    private void Update()
    {
        hintBoxCMP.fillAmount = Mathf.Lerp(hintBoxCMP.fillAmount, showHintBox ? 1 : 0, 0.04f);
    }
    private void FixedUpdate()
    {
        for (var i = 0; i < units.Length; i++)
        {
            if (IsCoolingdown(i))
            {
                units[i].cooldownCurrent -= Time.fixedDeltaTime;
                units[i].cooldownFillareaCMP.fillAmount = units[i].cooldownCurrent / units[i].cooldownTotal;
                if (!IsCoolingdown(i))
                {
                    units[i].cooldownFillarea.SetActive(false);
                }
            }
        }
    }

    private void LateUpdate()
    {
        // update button
        for (int i = 0; i < units.Length; i++)
        {
            bool enable = true;
            for (int j = 0; j < unitConfig.GetConfig(units[i].type).resourceNeeded.Length; j++)
            {
                if (unitConfig.GetConfig(units[i].type).resourceNeeded[j] > _res[(ResourceType)j])
                {
                    enable = false; break;
                }
            }
            SetEnable(i, enable);
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

    public void UpdateResource(Dictionary<ResourceType, int> res)
    {

        _res = res;
        for (int i = 0; i < resources.Length; i++)
        {
            resources[i].textCMP.text = "" + (int)(res[resources[i].type]);
        }
    }

    public void UpdateTimeBoard(float percent)
    {
        timeBoardCMP.fillAmount = Mathf.Clamp01(percent);
        if (percent >= 0.99f)
        {
            // show hint
            showHintBox = true;
            StartCoroutine("HideHintBox");
        }
    }
    IEnumerator HideHintBox(){
        yield return new WaitForSeconds(5.0f);
        showHintBox = false;
        StopCoroutine("HideHintBox");
    }
}
