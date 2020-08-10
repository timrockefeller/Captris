using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIScaler : MonoBehaviour
{
    // Start is called before the first frame update
    private CanvasScaler canvasScaler;
    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }
    private void Update()
    {

        canvasScaler.scaleFactor = Mathf.Lerp(canvasScaler.scaleFactor, Screen.height * 0.15f / 100f, 2 * Time.deltaTime);
    }
}
