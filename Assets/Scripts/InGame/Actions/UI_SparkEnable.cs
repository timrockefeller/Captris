using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI被启用时的效果
/// 从全白到全黑……
/// </summary>
public class UI_SparkEnable : MonoBehaviour
{
    private const int V = 4;
    public GameObject enableMaskPrefab;
    private GameObject enableMaskInstance;

    private Image enableMaskInstanceCMP;
    private bool running = true;
    private float Vtarget;
    private void OnEnable()
    {
        enableMaskInstance = Instantiate(enableMaskPrefab,transform);
        enableMaskInstanceCMP = enableMaskInstance.GetComponent<Image>();
        running = true;
        Vtarget = 5;
    }
    private void OnDisable()
    {
        if (enableMaskInstance != null) Destroy(enableMaskInstance);
        running = false;
    }
    private void Update()
    {
        if (running)
        {
            Vtarget = Mathf.Lerp(Vtarget, 0, V * Time.deltaTime);
            enableMaskInstanceCMP.fillAmount = Mathf.Clamp01(Vtarget);
            if (enableMaskInstanceCMP.fillAmount < 0.01f)
            {
                Destroy(enableMaskInstance);
                running = false;
            }
        }
    }
}
