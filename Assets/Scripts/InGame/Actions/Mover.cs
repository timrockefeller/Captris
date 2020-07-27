using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{

    private Vector3 target;
    private Vector3 subTarget;
    private bool isFinished = false;
    private void Start()
    {
        this.target = this.transform.position;
        
        this.subTarget = transform.position;
    }

    public void MoveTo(Vector3 pos,Action callback = null){
        this.target= pos;
        isFinished = false;
    }
    private void Update()
    {
        if (!isFinished)
        {
            this.subTarget = Vector3.Lerp(this.subTarget,this.target,0.005f);
            this.transform.position = Vector3.Lerp(this.transform.position, this.subTarget, 0.01f);
            if ((this.transform.position - this.target).magnitude < 0.01f)
            {
                this.transform.position = target;
                isFinished = true;
            }
        }
    }
}
