using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 4f;
    public float followSpeed = 2f;
    public Vector3 target;
    public bool enableMotionControl = false;
    // Start is called before the first frame update

    public PlayManager playManager;
    void Start()
    {
        // target = new Vector3(15, 0.5f, 15);
        playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo))
                //获取碰撞点的位置
                if (hitInfo.collider.tag == "Terrain" || hitInfo.collider.tag == "Piece" || hitInfo.collider.tag == "Wall")
                {
                    SetTarget(hitInfo.point);
                    playManager.SendEvent(PlayEventType.CONTROL_NAVIGATE);
                }

        }

        this.transform.position = Vector3.Lerp(this.transform.position, target, Time.deltaTime * followSpeed);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Debug.Log(Input.mousePosition);
        if (enableMotionControl)
        {
            if (Input.mousePosition.y > Screen.height * 0.9f) target += new Vector3(1, 0, 1) * Time.fixedDeltaTime * cameraSpeed;
            if (Input.mousePosition.y < Screen.height * 0.1f) target -= new Vector3(1, 0, 1) * Time.fixedDeltaTime * cameraSpeed;
            if (Input.mousePosition.x > Screen.width * 0.9f) target += new Vector3(1, 0, -1) * Time.fixedDeltaTime * cameraSpeed;
            if (Input.mousePosition.x < Screen.width * 0.1f) target -= new Vector3(1, 0, -1) * Time.fixedDeltaTime * cameraSpeed;
        }

    }
    public void SetTarget(Vector3 t)
    {
        target = t;
        target.y = 0.5f;
    }

    // TODO 
    public void DoVibrate(Vector3 source)
    {
        float magnitude = (source - target).magnitude;
        // magnitude * ;
        Debug.Log("[INFO] CameraController.DoVibrate() called at " + source + "!");
    }
}
