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
        rigidbodyCMP = GetComponent<Rigidbody>();
    }
    private Rigidbody rigidbodyCMP;
    public GameObject target = null;
    void FixedUpdate()
    {
        if (target == null)
        {
            transform.position += transform.forward * speed * Time.fixedDeltaTime;
            Quaternion TargetRotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.fixedDeltaTime * gravity);

            curTime += Time.fixedDeltaTime;
            if (curTime > lifeTime)
            {
                //TODO Damage
                Destroy(gameObject);
            }
            rigidbodyCMP.useGravity = true;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.fixedDeltaTime * speed / 2);
            rigidbodyCMP.useGravity = false;
        }
    }
}
