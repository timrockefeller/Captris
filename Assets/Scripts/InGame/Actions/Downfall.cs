using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downfall : MonoBehaviour
{

    private Vector3 SolidPosition;
    private bool isFinished = false;
    private void Start()
    {
        this.SolidPosition = this.transform.position;
        this.transform.position += new Vector3(0, 1, 0);
    }
    private void Update()
    {
        if (!isFinished)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, this.SolidPosition, 0.03f);
            if (Mathf.Abs(this.transform.position.y - this.SolidPosition.y) < 0.01)
            {
                this.transform.position = SolidPosition;
                isFinished = true;
            }
        }
    }
}
