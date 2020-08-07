using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 speed;
    public float speedY;
    public float maxSpeed;
    public float acc;
    public float drag;

    [Tooltip("站在空区域上被减速(或扣血)")]// decrease HP
    [Range(0, 1)]
    public float mistCost;
    private bool onGround = false;
    private bool onMist = false;
    private Rigidbody rigidbodyComponent;
    private CameraController cameraController;
    private PlayerStatsManager playerStatsManager;
    private PlayManager playManager;

    [Header("Mist Damage")]
    public float mistHurtingPeriod = 0.5f;
    public float mistDamage = 10f;
    private float mistHurtingTime = 0;
    private bool onWall;

    void Start()
    {
        rigidbodyComponent = gameObject.GetComponent<Rigidbody>();
        cameraController = GameObject.Find("CamPos").GetComponent<CameraController>();
        playerStatsManager = GameObject.Find("PlayerStatsManager").GetComponent<PlayerStatsManager>();
        playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();



        cameraController.SetTarget(transform.position);//  offset to tips
        // StartCoroutine(CamMoveToTips());
    }

    // IEnumerator CamMoveToTips()
    // {
    //     yield return new WaitForSeconds(1);
    //     Ray ray = Camera.main.ScreenPointToRay(new Vector2(200, 100));
    //     RaycastHit hitInfo = new RaycastHit();
    //     if (Physics.Raycast(ray, out hitInfo))
    //         if (hitInfo.collider.tag == "Terrain" || hitInfo.collider.tag == "Piece" || hitInfo.collider.tag == "Wall")
    //         {
    //             cameraController.SetTarget(hitInfo.point);
    //             playManager.SendEvent(PlayEventType.CONTROL_NAVIGATE);
    //         }
    // }
    void FixedUpdate()
    {
        // controller
        Vector3 force = new Vector3(0, 0, 0);
        // force += new Vector3(1, 0, -1) * Input.GetAxis("Horizontal");
        // force += new Vector3(1, 0, 1) * Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.W)) force += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.S)) force -= new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.A)) force -= new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.D)) force += new Vector3(0, 0, -1);

        if (speed.magnitude > 0) cameraController.SetTarget(transform.position);
        speed += acc * Vector3.Normalize(force) * Time.deltaTime * (onMist || onWall ? mistCost : 1);
        if (!onGround)
        {
            speedY += -1 * 3 * Time.deltaTime;
            speedY = Mathf.Clamp(speedY, -maxSpeed, 2 * maxSpeed);
        }
        else
        {
            // rigidbodyComponent.velocity -= new Vector3(0, rigidbodyComponent.velocity.y, 0);
            speedY = 0;
            if (!onWall && Input.GetButton("Jump"))
            {
                speedY = 3.3f * (onMist ? mistCost : 1);
            }
        }
        speed = Vector3.ClampMagnitude(speed, maxSpeed * (onMist || onWall ? mistCost : 1));
        transform.position += (speed + new Vector3(0, speedY, 0)) * Time.deltaTime;
        if (force.magnitude <= 0)
            speed = Vector3.ClampMagnitude(speed, Mathf.Clamp(speed.magnitude - drag * Time.deltaTime, 0, speed.magnitude));
        else
        {
            playManager.SendEvent(PlayEventType.PLAYER_MOVE);

            transform.rotation = Quaternion.LookRotation(force, Vector3.up);
        }
        // damage
        if (onMist)
        {
            mistHurtingTime += Time.fixedDeltaTime;
            if (mistHurtingTime > mistHurtingPeriod)
            {
                //do damage 
                playerStatsManager.TakeDamage(mistDamage);
                mistHurtingTime -= mistHurtingPeriod;
            }
        }
        else
        {
            mistHurtingTime = 0;
        }
        if (!playerStatsManager.IsAlive())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.down, Vector3.forward), Time.fixedUnscaledDeltaTime * 10);
        }
    }


    void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "Piece")
        {
            this.transform.position += new Vector3(0, 0.1f, 0) * Time.deltaTime;
            this.onGround = true;
            this.onMist = false;
            onWall = false;
        }
        if (other.collider.tag == "Terrain")
        {
            //die? 碰到空区域
            this.transform.position += new Vector3(0, 0.1f, 0) * Time.deltaTime;
            this.onGround = true;
            this.onMist = true;
        }
        if (other.collider.tag == "Wall")
        {
            this.onWall = true;
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "Piece")
        {
            this.onGround = false;
        }
        if (other.collider.tag == "Wall")
        {
            onWall = false;
        }
    }
}
