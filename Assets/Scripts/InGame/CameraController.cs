using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 0.02f;
    public Vector3 target;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.Log(hitInfo.collider);
                //获取碰撞点的位置
                if (hitInfo.collider.tag == "Terrain"||hitInfo.collider.tag =="Piece")
                {
                    target = hitInfo.point;
                    target.y = 0.5f;
                }
            }
        }
        this.transform.position = Vector3.Lerp(this.transform.position, target, cameraSpeed);
    }
}
