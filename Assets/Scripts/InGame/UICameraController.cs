using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraController : MonoBehaviour
{
    public GameObject[] nextPieceInstance;


    public float nextPieceTargetRotateSpeed = 2.4f;

    [Header("Referrences")]
    public GameObject previewPos;
    public GameObject previewPos1;
    public GameObject previewPos2;
    private Quaternion nextPieceTargetRotation;
    private int nextPieceTargetRotationCount = 0;
    Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
        nextPieceInstance = new GameObject[3];
    }

    public bool DoRotate(bool isClockwise)
    {
        if (isClockwise) nextPieceTargetRotationCount = (nextPieceTargetRotationCount + 1) % 4;
        else nextPieceTargetRotationCount = (nextPieceTargetRotationCount + 3) % 4;
        nextPieceTargetRotation = Quaternion.Euler(0, -90 * nextPieceTargetRotationCount + nextPieceTargetRotation.y, 0) * Quaternion.identity;
        return true;
    }
    public bool SetInstance(GameObject nextPiecePrefab1, GameObject nextPiecePrefab2 = null, GameObject nextPiecePrefab3 = null)
    {
        if (nextPieceInstance[0] != null)
            Destroy(nextPieceInstance[0]);
        nextPieceInstance[0] = Instantiate(nextPiecePrefab1, previewPos.transform);
        nextPieceTargetRotation = nextPieceInstance[0].transform.rotation;
        nextPieceTargetRotationCount = 0;
        if (nextPiecePrefab2 != null)
        {
            if (nextPieceInstance[1] != null)
            {
                Destroy(nextPieceInstance[1]);
            }
            nextPieceInstance[1] = Instantiate(nextPiecePrefab2, previewPos1.transform);
        }
        if (nextPiecePrefab3 != null)
        {
            if (nextPieceInstance[2] != null)
            {
                Destroy(nextPieceInstance[2]);
            }
            nextPieceInstance[2] = Instantiate(nextPiecePrefab3, previewPos2.transform);
        }
        // nextPieceInstance
        return true;
    }

    void LateUpdate()
    {

        if (nextPieceInstance[0] != null)
            nextPieceInstance[0].transform.rotation = Quaternion.Slerp(nextPieceInstance[0].transform.rotation, nextPieceTargetRotation, nextPieceTargetRotateSpeed * Time.deltaTime);
        previewPos.transform.position = cam.ScreenToWorldPoint(new Vector3(100, 200, cam.nearClipPlane)) + transform.forward;
        previewPos1.transform.position = cam.ScreenToWorldPoint(new Vector3(180, 180, cam.nearClipPlane)) + transform.forward;
        previewPos2.transform.position = cam.ScreenToWorldPoint(new Vector3(210, 180, cam.nearClipPlane)) + transform.forward;
    }
}
