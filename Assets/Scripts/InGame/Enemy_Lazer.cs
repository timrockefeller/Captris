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

    private float speed;
    private float torque;

    [Header("Attack")]
    public float attackCD;
    private float currentCD;
    /// <summary>
    /// Random Generated
    /// </summary>
    private float size;

    public GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = null;
        size = RD.NextFloat() * 0.25f + 0.8f;
    }
    /// 
    /// <summary>
    /// 和玩家的水平向量
    /// </summary>
    Vector3 HorizonVectorToPlayer()
    {
        if (player != null)
        {
            Vector3 c = player.transform.position - transform.position;
            return new Vector3(c.x, 0, c.y);
        }
        return new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    }
    /// <summary>
    /// 和玩家的水平距离
    /// </summary>
    float DistanceToPlayer()
    {
        return HorizonVectorToPlayer().magnitude;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.fixedDeltaTime * torque * size);


        // attack
        if (currentCD < attackCD)
        {
            if (DistanceToPlayer() < 5)
                currentCD += Time.fixedDeltaTime;
            else
                currentCD += Time.fixedDeltaTime * 0.3f;

        }
        else
        {
            DoAttack();
            currentCD -= attackCD;
        }

    }

    /// <summary>
    /// 投弹设备启动！
    /// </summary>
    private void DoAttack()
    {

        GameObject bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.LookRotation(HorizonVectorToPlayer(), Vector3.up));
        bullet.transform.SetParent(null);
    }




    public void SetSize(float size)
    {
        this.size = size;
        // TrailRenderer renderer = GetComponent<TrailRenderer>();
        // renderer.widthCurve.MoveKey()
    }
}
