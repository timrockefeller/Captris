using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 speed;
    public float speedY;
    public float maxSpeed;
    public float acc;
    public float drag;
    [Tooltip("站在空区域上被减速(或扣血)")]// TODO  decrease HP
    [Range(0, 1)]
    public float mistCost;
    private bool onGround = false;
    private bool onMist = false;
    private Rigidbody rigidbodyComponent;
    void Start()
    {
        rigidbodyComponent = gameObject.GetComponent<Rigidbody>();
    }
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
        speed += acc * Vector3.Normalize(force) * Time.deltaTime * (onMist ? mistCost : 1);
        if (!onGround)
        {
            speedY += -1 * 3 * Time.deltaTime;
            speedY = Mathf.Clamp(speedY, -maxSpeed, 2 * maxSpeed);
        }
        else
        {
            speedY = 0;
            if (Input.GetButton("Jump"))
            {
                speedY = 4 * (onMist ? mistCost : 1);
            }
        }
        speed = Vector3.ClampMagnitude(speed, maxSpeed * (onMist ? mistCost : 1));
        transform.position += (speed + new Vector3(0, speedY, 0)) * Time.deltaTime;
        if (force.magnitude <= 0)
            speed = Vector3.ClampMagnitude(speed, Mathf.Clamp(speed.magnitude - drag * Time.deltaTime, 0, speed.magnitude));

    }



    void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "Piece")
        {
            this.transform.position += new Vector3(0, 0.1f, 0) * Time.deltaTime;
            this.onGround = true;
            this.onMist = false;
        }
        if (other.collider.tag == "Terrain")
        {
            //TODO die? 碰到空区域
            this.transform.position += new Vector3(0, 0.1f, 0) * Time.deltaTime;
            this.onGround = true;
            this.onMist = true;
        }

    }
    void OnCollisionExit(Collision other)
    {
        if (other.collider.tag == "Piece")
        {
            this.onGround = false;
        }

    }
}
