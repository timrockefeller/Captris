using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lazer : MonoBehaviour
{

    public GameObject player;

    [Header("Motivate")]
    public float maxSpeed;
    public float maxAcc;
    public float maxTorque;

    [Header("Attack")]
    public float attackCD;

    /// <summary>
    /// Random Generated
    /// </summary>
    private float size;

    private float speed;
    private float torque;
    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = null;
        size = RD.NextFloat() * 0.25f + 0.8f;
    }

    float DistanceToPlayer()
    {
        if (player != null)
        {
            return (player.transform.position - transform.position).magnitude;
        }
        return Mathf.Infinity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
            if (player == null) { return; }
        }

        // movement

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
        torque = maxTorque * (1 - speed / maxSpeed);
        Quaternion TargetRotation = Quaternion.LookRotation(player.transform.position - transform.position + Vector3.up * 2, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * torque * size);


        // attack


    }


    private void DoAttack()
    {

    }




    public void SetSize(float size)
    {
        this.size = size;
        // TrailRenderer renderer = GetComponent<TrailRenderer>();
        // renderer.widthCurve.MoveKey()
    }
}
