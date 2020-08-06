using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Periodicitily Jumping & 
/// </summary>
public class Enemy_Giant : MonoBehaviour
{
    private const float RETIRE_INIT = 0.5f;
    public GameObject player;

    [Header("Motivate")]
    [Range(0.1f, 3f)]
    public float jumpCD = 2;
    public float curJumpCD = 0;
    [Range(0.1f, 30f)]
    public float jumpSpeed = 2;
    [Range(0.1f, 30f)]
    public float jumpHeight = 2;
    public float torque = 2;
    public int breathCount;

    public bool onGround;

    Rigidbody rigidbodyCMP;
    CameraController mainCameraCMP;
    Enemy_Giant_Face enemy_Giant_Face;

    private bool _h_onGround = false;

    public Vector3 speed;

    // Bouince

    private float retireTime = RETIRE_INIT;
    private int curBreathCount;

    private void Start()
    {
        player = null;

        rigidbodyCMP = GetComponent<Rigidbody>();
        enemy_Giant_Face = transform.Find("Face").GetComponent<Enemy_Giant_Face>();
        mainCameraCMP = GameObject.Find("MainCamera").GetComponent<CameraController>();
        curBreathCount = breathCount;
    }


    private void Update()
    {
        transform.position += speed * Time.deltaTime;
    }
    private void FixedUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
            return;
        }
        if (!_h_onGround && onGround && mainCameraCMP) mainCameraCMP.DoVibrate(transform.position);

        if (onGround)
        {
            if (curBreathCount <= 0)
                DoRotate();

            speed = Vector3.zero;
            curJumpCD += Time.deltaTime;
            if (curJumpCD > jumpCD)
            {
                retireTime = RETIRE_INIT;
                // Do jump


                if (!enemy_Giant_Face.facingWall)
                {
                    if (curBreathCount <= 0)
                    {
                        DoBigJump();
                        curBreathCount = breathCount;
                    }
                    else
                    {
                        DoJump();
                        curBreathCount--;
                    }
                }
                else
                    DoBackJump();

                curJumpCD = 0;
                onGround = false;


            }

        }
        else
        {

            speed += Vector3.down * 9.8f * Time.fixedDeltaTime;
            DoRotate();

        }

        retireTime = Mathf.Max(0, retireTime - Time.fixedDeltaTime);

        _h_onGround = onGround;
    }
    private void DoRotate()
    {
        // rotate
        Vector3 PtG = player.transform.position - transform.position;
        Quaternion TargetRotation = Quaternion.LookRotation(new Vector3(PtG.x, 0, PtG.z).normalized, Vector3.up);
        transform.localRotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.fixedDeltaTime * torque);
    }
    private void DoBigJump()
    {
        Vector3 PtG = player.transform.position - transform.position;
        speed = transform.forward * jumpSpeed * Mathf.Sqrt(PtG.magnitude) + Vector3.up * jumpHeight * 2;
    }

    private void DoBackJump()
    {
        speed = -transform.forward * jumpSpeed / 4 + Vector3.up * jumpHeight;
    }

    private void DoJump()
    {
        speed = transform.forward * jumpSpeed + Vector3.up * jumpHeight;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            // TODO cast damage to player
        }

        if (other.collider.tag == "Terrain" || other.collider.tag == "Piece")
        {
            if (retireTime <= 0)
                onGround = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "Terrain" || other.collider.tag == "Piece")
        {
            onGround = false;
        }
    }
}
