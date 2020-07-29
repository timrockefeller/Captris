using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于出现时期
/// </summary>
public class Downfall : MonoBehaviour
{

    private Vector3 SolidPosition;
    public float fallspeed = 4f;
    private bool isFinished = false;
    private void Start()
    {
        this.SolidPosition = this.transform.position;

        if (transform.rotation == Quaternion.identity)
            this.transform.position += new Vector3(0, 1, 0);
    }
    private void Update()
    {
        if (!isFinished)
            this.transform.position = Vector3.Lerp(this.transform.position, this.SolidPosition, Time.deltaTime * fallspeed);
    }
    private void FixedUpdate()
    {
        if (!isFinished)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, this.SolidPosition, Time.fixedDeltaTime * fallspeed);
            if (Mathf.Abs(this.transform.position.y - this.SolidPosition.y) < 0.01)
            {
                this.transform.position = SolidPosition;
                isFinished = true;
            }
        }
    }
}
