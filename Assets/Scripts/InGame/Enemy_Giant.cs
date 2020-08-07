using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Periodicitily Jumping & 
/// </summary>
[RequireComponent(typeof(Health))]
public class Enemy_Giant : MonoBehaviour
{
    /// 引用组件与对象
    [Header("References Objects")]
    /// <summary>
    /// 爆炸特效，大跳后触发生成
    /// </summary>
    public GameObject explotionPrefab;
    /// <summary>
    /// 场上玩家的引用
    /// </summary>
    private GameObject player;
    /// <summary>
    /// 全局对象
    /// </summary>
    CameraController mainCameraCMP;
    /// <summary>
    /// 子对象，判定脸贴墙用
    /// </summary>
    Enemy_Giant_Face enemy_Giant_Face;
    /// <summary>
    /// 附加组件：血量
    /// </summary>
    private Health health;
    /// <summary>
    /// 全局对象
    /// </summary>
    private PlayManager playManager;
    /// <summary>
    /// 全局对象
    /// </summary>
    private PlayerStatsManager playerStatsManager;

    [Header("Motivate")]
    [Tooltip("跳跃冷却")]
    [Range(0.1f, 3f)]
    public float jumpCD = 2;
    /// <summary>
    /// 当前经过的秒数
    /// </summary>
    [ReadOnly]
    [SerializeField]
    private float curJumpCD = 0;
    [Tooltip("跳跃水平速度base")]
    [Range(0.1f, 30f)]
    public float jumpSpeed = 2;
    [Tooltip("跳跃高度base")]
    [Range(0.1f, 30f)]
    public float jumpHeight = 2;
    [Tooltip("转向力矩")]
    public float torque = 2;

    [Tooltip("大跳冷却时间")]
    public int breathCount;
    /// <summary>
    /// 目前大跳剩余的冷却时间
    /// </summary>
    [ReadOnly]
    [SerializeField]
    private int curBreathCount;

    /// <summary>
    /// 是否在地面上
    /// </summary>
    [ReadOnly]
    [SerializeField]
    private bool onGround;
    /// <summary>
    /// 上一帧的onGround值，以判定落地
    /// </summary>
    private bool _h_onGround = false;

    /// <summary>
    /// 当前受控速度
    /// </summary>   
    [ReadOnly]
    [SerializeField]
    private Vector3 speed;

    // Bouince
    /// <summary>
    /// 判定OnGround时防止Collider调用问题
    /// </summary>
    private float retireTime = RETIRE_INIT;

    private const float RETIRE_INIT = 0.5f;

    /// <summary>
    /// 跳起时动效参数
    /// 
    /// 本质是一个附加点受其惯性影响
    /// </summary>
    private float scaleCtrlPosY
    {
        get
        {
            return _scaleCtrlPosY;
        }
        set
        {
            ///限制在±1
            _scaleCtrlPosY = Mathf.Clamp(value, transform.position.y - 1f, transform.position.y + 1f);
        }
    }
    private float _scaleCtrlPosY;
    private float scaleCtrlPosXY = 1;

    /// <summary>
    /// 死亡动画参数
    /// </summary>
    private Vector3 deathTargetScale = new Vector3(1.5f, 0.01f, 1.5f);
    private bool _deathAnimMutex = true;

    private void Start()
    {
        // 本地初始化项
        player = null;
        curBreathCount = breathCount;
        this.scaleCtrlPosY = transform.position.y;

        // 子组件
        enemy_Giant_Face = transform.Find("Face").GetComponent<Enemy_Giant_Face>();
        health = GetComponent<Health>();

        // 全局组件
        mainCameraCMP = GameObject.Find("CamPos").GetComponent<CameraController>();
        playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        playerStatsManager = GameObject.Find("PlayerStatsManager").GetComponent<PlayerStatsManager>();
    }

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

        if (onGround)
        {
            if (curBreathCount <= 0 && curJumpCD > jumpCD / 2.0f)
            {
                DoRotate();
                scaleCtrlPosY = transform.position.y + 0.5f;
            }

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
            speed.y = Mathf.Clamp(speed.y, -5, Mathf.Infinity);
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


        playManager.SendEvent(PlayEventType.PLAYER_KILL);
        playManager.SendEvent(PlayEventType.PLAYER_KILL_GIANT);
        // scaleCtrlPosY = transform.position.y + 1;
        speed = Vector3.zero;
        yield return new WaitForSeconds(2);
        deathTargetScale = new Vector3(0f, 0f, 0f);
        int goldNum = 10;
        while (goldNum-- > 0)
        {
            playManager.GainResource(ResourceType.GOLD);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Terrain" || other.collider.tag == "Piece")
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
            // cast damage to player

        }

        if (other.collider.tag == "Terrain" || other.collider.tag == "Piece")
        {
            if (retireTime <= 0)
                onGround = true;

        }

    }
    private IEnumerator DoExplode()
    {
        // 震动一波摄像头
        mainCameraCMP.DoVibrate(transform.position);
        // 伤敌80，自损15
        health.DoAttack(15);

        // 爆炸部分
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
