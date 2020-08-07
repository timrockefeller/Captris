using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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

        [Tooltip("回调函数")]
        public UnityEvent callback;
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

    static Color COLOR_NORMAL = new Color(255 / 255F, 255 / 255F, 255 / 255F, 97 / 255F);

    static Color COLOR_HIGHLIGHT = new Color(255 / 255F, 255 / 255F, 255 / 255F, 170 / 255F);
    static Color COLOR_HIDDEN = new Color(255 / 255F, 255 / 255F, 255 / 255F, 0 / 255F);
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
                UpdateText();
                if (curNum >= missions[curMission].num)
                    // next mission
                    StartCoroutine(NextMission());
            }
        }
    }

    void UpdateText()
    {
        if (curMission < missions.GetLength(0))
        {

            var texts = missions[curMission].text.Split('|');
            var finalText = texts[0] + (missions[curMission].num > 1 ? " (" + curNum + "/" + missions[curMission].num + ")" : "");
            int i = 1;
            while (i < texts.GetLength(0))
            {
                finalText += "\n" + texts[i];
                i++;
            }
            this.missionTextCMP.text = finalText;
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
        missions[curMission].callback.Invoke();
        inMission = false;
        curMission++;
        targetFill = 1;
        targetColor = COLOR_HIGHLIGHT;
        yield return new WaitForSeconds(1.5f);
        curNum = 0;
        targetColor = COLOR_HIDDEN;
        yield return new WaitForSeconds(1);
        targetFill = 0;
        missionTickCMP.fillAmount = 0;
        missionTickCMP.color = COLOR_HIDDEN;
        targetColor = COLOR_NORMAL;
        UpdateText();
        if (curMission < missions.GetLength(0))
            inMission = true;
    }
}
