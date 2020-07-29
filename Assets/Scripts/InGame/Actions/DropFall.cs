using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFall : MonoBehaviour
{
    // Start is called before the first frame update
    public float acc = 3;
    public float threshold = -50;
    private bool isFalling = false;
    private float speed = 0;
    private Action tmpCallback = null;
    private GameObject instance;
    public void Fall(Action callback = null)
    {
        instance = Instantiate(gameObject);
        speed = 0;
        isFalling = true;
        if (callback != null)
            this.tmpCallback = callback;


    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFalling)
        {
            instance.transform.position += new Vector3(0, -speed * Time.fixedDeltaTime, 0);
            speed += acc * Time.fixedDeltaTime;
            if (instance.transform.position.y < threshold)
            {
                if (tmpCallback != null) tmpCallback();
                Destroy(instance);
                isFalling = false;
            }
        }
    }
}
