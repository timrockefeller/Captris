using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMotivation : MonoBehaviour
{

    public float speed;
    public float gravity;

    public float lifeTime;
    private float curTime = 0;
    // Update is called once per frame
    void Start()
    {
        curTime = 0;
    }
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        Quaternion TargetRotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * gravity);

    }
    void FixedUpdate()
    {
        curTime += Time.fixedDeltaTime;
        if (curTime > lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
