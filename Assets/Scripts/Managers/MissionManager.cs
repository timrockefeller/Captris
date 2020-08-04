using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
public class MissionManager : MonoBehaviour
{

    [Serializable]
    public struct MissionSingleton
    {
        [Tooltip("任务简介")]
        public string text;
        [Tooltip("监听的事件名")]
        public PlayEventType condition;
        [Tooltip("需要达成的次数")]
        public int num;
    }
    public MissionSingleton[] missions;


    private int curMission;
    private int curNum;

    [Header("UI Reference")]
    public GameObject missionText;
    private Text missionTextCMP;

    public GameObject missionTick;
    private Image missionTickCMP;


    /////////////
    /// Animations
    private Color targetColor;
    private float targetFill;
    private float _colorSpeed = 4F;
    private float _fillSpeed = 2F;

    static Color COLOR_NORMAL = new Color(255/255F, 255/255F, 255/255F, 97/255F);
    static Color COLOR_HIDDEN = new Color(255/255F, 255/255F, 255/255F, 0/255F);
    private void Start()
    {
        missionTextCMP = missionText.GetComponent<Text>();
        missionTickCMP = missionTick.GetComponent<Image>();

        targetFill = 0;
        targetColor = COLOR_NORMAL;
        missionTextCMP.color = COLOR_NORMAL;
        missionTickCMP.color = COLOR_NORMAL;
        curNum = 0;
        UpdateText();
        inMission = true;
    }

    private void Update()
    {
        // animations
        missionTextCMP.color = Color.Lerp(missionTextCMP.color, targetColor, Time.deltaTime * _colorSpeed);
        missionTickCMP.color = missionTextCMP.color;
        missionTickCMP.fillAmount = Mathf.Lerp(missionTickCMP.fillAmount, targetFill, Time.deltaTime * _fillSpeed);
    }


    public void AckEvent(PlayEventType t)
    {
        if (!inMission) return;
        if (curMission < missions.GetLength(0))
        {
            if (missions[curMission].condition == t)
            {
                curNum++;
                if (curNum >= missions[curMission].num)
                {
                    // next mission
                    StartCoroutine(NextMission());
                }
            }
        }
    }

    void UpdateText()
    {
        if (curMission < missions.GetLength(0))
        {
            this.missionTextCMP.text = missions[curMission].text + (missions[curMission].num > 1 ? " (" + curNum + "/" + missions[curMission].num + ")" : "");
        }
        else
        {
            this.missionTextCMP.text = "";
            targetFill = 0;
        }
    }
    private bool inMission = false;
    IEnumerator NextMission()
    {
        inMission = false;
        curMission++;
        targetFill = 1;
        yield return new WaitForSeconds(1);
        curNum = 0;
        targetColor = COLOR_HIDDEN;
        yield return new WaitForSeconds(1);
        targetFill = 0;
        missionTickCMP.fillAmount = 0;
        missionTickCMP.color = COLOR_HIDDEN;
        inMission = true;
        targetColor = COLOR_NORMAL;
        UpdateText();
    }
}