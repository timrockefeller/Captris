using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 speed;
    public float speedy;
    public float maxSpeed;
    public float acc;
    public float drag;
    private bool onGround = false;

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
        if (Input.GetKey(KeyCode.W)) force += new Vector3(1, 0, 1);
        if (Input.GetKey(KeyCode.S)) force -= new Vector3(1, 0, 1);
        if (Input.GetKey(KeyCode.A)) force -= new Vector3(1, 0, -1);
        if (Input.GetKey(KeyCode.D)) force += new Vector3(1, 0, -1);
        speed += acc * Vector3.Normalize(force) * Time.deltaTime;
        if (!onGround)
        {
            speedy += -1 * acc * Time.deltaTime;
            speedy = Mathf.Clamp(speedy, -maxSpeed, 2 * maxSpeed);
        }
        else
        {
            speedy = 0;
            if (Input.GetButton("Jump"))
            {
                speedy = 4;
            }
        }
        speed = Vector3.ClampMagnitude(speed, maxSpeed);
        transform.position += (speed + new Vector3(0, speedy, 0)) * Time.deltaTime;
        if (force.magnitude <= 0)
            speed = Vector3.ClampMagnitude(speed, Mathf.Clamp(speed.magnitude - drag * Time.deltaTime, 0, speed.magnitude));

    }



    void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "Piece")
        {
            this.transform.position += new Vector3(0, 0.1f, 0) * Time.deltaTime;
            this.onGround = true;
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