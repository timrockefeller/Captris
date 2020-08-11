using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public GameObject cameraInstance;




    private void Start()
    {
        cameraInstance = GameObject.Find("MainCamera");
    }


    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(cameraInstance.transform.position - transform.position, Vector3.up);
    }



}
