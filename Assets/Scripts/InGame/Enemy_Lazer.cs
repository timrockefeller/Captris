using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lazer : MonoBehaviour
{

    public GameObject player;

    [Header("Motivate Properties")]
    public float maxSpeed;
    public float maxAcc;
    public float maxTorque;

    /// <summary>
    /// Random Generated
    /// </summary>
    public float size;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        player = null;
        size = RD.NextFloat()*0.25f+0.8f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
            if (player == null) { return; }
        }


        transform.position += transform.forward * speed * Time.fixedDeltaTime;
        if (Vector3.Dot(transform.forward, player.transform.position - transform.position) > 0)
        {
            //speed up
            speed += maxAcc * Time.fixedDeltaTime;
        }
        else
        {
            //slow down
            speed -= maxAcc * Time.fixedDeltaTime;
        }
        speed = Mathf.Clamp(speed, size * maxSpeed / 2, maxSpeed * size);

        Quaternion TargetRotation = Quaternion.LookRotation(player.transform.position - transform.position + Vector3.up * 2, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * maxTorque * size);


        // attack

    }
    public void SetSize(float size)
    {
        this.size = size;
        // TrailRenderer renderer = GetComponent<TrailRenderer>();
        // renderer.widthCurve.MoveKey()
    }
}
