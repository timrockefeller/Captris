using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 4f;
    public float followSpeed = 0.02f;
    public Vector3 target;
    public bool enableMotionControl = false;
    // Start is called before the first frame update
    void Start()
    {
        // target = new Vector3(15, 0.5f, 15);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Input.mousePosition);
        if (enableMotionControl)
        {
            if (Input.mousePosition.y > Screen.height * 0.9f) target += new Vector3(1, 0, 1) * Time.deltaTime * cameraSpeed;
            if (Input.mousePosition.y < Screen.height * 0.1f) target -= new Vector3(1, 0, 1) * Time.deltaTime * cameraSpeed;
            if (Input.mousePosition.x > Screen.width * 0.9f) target += new Vector3(1, 0, -1) * Time.deltaTime * cameraSpeed;
            if (Input.mousePosition.x < Screen.width * 0.1f) target -= new Vector3(1, 0, -1) * Time.deltaTime * cameraSpeed;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo))
                //获取碰撞点的位置
                if (hitInfo.collider.tag == "Terrain" || hitInfo.collider.tag == "Piece")
                    SetTarget(hitInfo.point);

        }
        this.transform.position = Vector3.Lerp(this.transform.position, target, followSpeed);
    }
    public void SetTarget(Vector3 t)
    {
        target = t;
        target.y = 0.5f;
    }
}
