using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Periodicitily Jumping & 
/// </summary>
[RequireComponent(typeof(Health))]
public class Enemy_Giant : MonoBehaviour
{
    private const float RETIRE_INIT = 0.5f;
    public GameObject player;
    public GameObject explotionPrefab;
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

    public int curBreathCount;
    public bool onGround;

    Rigidbody rigidbodyCMP;
    CameraController mainCameraCMP;
    Enemy_Giant_Face enemy_Giant_Face;

    private bool _h_onGround = false;

    public Vector3 speed;

    // Bouince

    private float retireTime = RETIRE_INIT;
    private float _scaleCtrlPosY;
    private float scaleCtrlPosY
    {
        get
        {
            return _scaleCtrlPosY;
        }
        set
        {
            _scaleCtrlPosY = Mathf.Clamp(value, transform.position.y - 1f, transform.position.y + 1f);
        }
    }
    private float scaleCtrlPosXY = 1;

    private Health health;
    private PlayManager playManager;
    private void Start()
    {
        player = null;

        rigidbodyCMP = GetComponent<Rigidbody>();
        enemy_Giant_Face = transform.Find("Face").GetComponent<Enemy_Giant_Face>();
        mainCameraCMP = GameObject.Find("MainCamera").GetComponent<CameraController>();

        playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        curBreathCount = breathCount;
        this.scaleCtrlPosY = transform.position.y;

        health = GetComponent<Health>();
    }
    private Vector3 deathTargetScale = new Vector3(1.5f, 0.01f, 1.5f);

    private void Update()
    {
        if (health.IsAlive())
        {
            scaleCtrlPosXY = Mathf.Lerp(scaleCtrlPosXY, onGround ? Mathf.Clamp01((scaleCtrlPosY - transform.position.y) / 2) : 0, Time.deltaTime * 10);
            transform.position += speed * Time.deltaTime;
            transform.localScale = new Vector3(1 + scaleCtrlPosXY,
             (-scaleCtrlPosY + transform.position.y) / 2F + 1,
             1 + scaleCtrlPosXY);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, deathTargetScale, 10 * Time.deltaTime);
        }
    }

    private bool _deathAnimMutex = true;
    private void FixedUpdate()
    {
        if (!health.IsAlive())
        {
            if (_deathAnimMutex)
                StartCoroutine(DiedAnim());
            _deathAnimMutex = false;
            return;
        }
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
                while (curBreathCount < 0)
                {
                    curBreathCount += breathCount;
                }

                if (!enemy_Giant_Face.facingWall)
                {
                    if (curBreathCount <= 0)
                    {
                        DoBigJump();

                    }
                    else
                    {
                        DoJump();
                    }
                }
                else
                    DoBackJump();

                curBreathCount--;
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

        scaleCtrlPosY = Mathf.Lerp(scaleCtrlPosY, transform.position.y, Time.deltaTime * 2);
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
    private IEnumerator DiedAnim()
    {
        // scaleCtrlPosY = transform.position.y + 1;
        speed = Vector3.zero;
        yield return new WaitForSeconds(2);
        deathTargetScale = new Vector3(0f,0f,0f);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Terrain")
        {
            if (curBreathCount < 0)
            {
                StartCoroutine(DoExplode());
                curBreathCount = breathCount;
            }
        }
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
    private IEnumerator DoExplode()
    {

        int totalExplotions = 5;
        while (totalExplotions-- > 0)
        {
            Vector2 varing = RD.NextPositionf(1, 1);
            Vector3 pos = transform.position + new Vector3(varing.x - 0.5f, -transform.localScale.y / 2 + 0.2f, varing.y - 0.5f);
            Instantiate(explotionPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
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
