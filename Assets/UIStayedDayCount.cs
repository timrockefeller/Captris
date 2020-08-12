using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIStayedDayCount : MonoBehaviour
{

    PlayManager playManager;
    Text text;
    void Start()
    {
        text = GetComponent<Text>();
        playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        // PlayManager
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "你坚持了：" + playManager.dayCount + " 天";
    }
}
