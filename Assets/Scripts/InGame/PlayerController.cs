using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 speed;
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
        force += new Vector3(1, 0, -1) * Input.GetAxis("Horizontal");
        force += new Vector3(1, 0, 1) * Input.GetAxis("Vertical");
        speed += acc * Vector3.Normalize(force);
        if (!onGround)
        {
            speed += new Vector3(0, -1, 0) * acc / 2.0f;
        }else {
            speed.y = 0;
        }
        speed = Vector3.ClampMagnitude(speed, maxSpeed);
        transform.position += speed * Time.deltaTime;
        speed = Vector3.ClampMagnitude(speed, Mathf.Clamp(speed.magnitude - drag, 0, speed.magnitude));

    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Piece")
        {
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