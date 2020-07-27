using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraController : MonoBehaviour
{
    public GameObject nextPieceInstance;


    public float nextPieceTargetRotateSpeed = 0.02f;

    [Header("Referrences")]
    public GameObject previewPos;
    public GameObject previewPos1;
    public GameObject previewPos2;
    private Quaternion nextPieceTargetRotation;
    private int nextPieceTargetRotationCount = 0;
    void Start()
    {

    }

    public bool DoRotate(bool isClockwise)
    {
        if (isClockwise) nextPieceTargetRotationCount = (nextPieceTargetRotationCount + 1) % 4;
        else nextPieceTargetRotationCount = (nextPieceTargetRotationCount + 3) % 4;
        nextPieceTargetRotation = Quaternion.Euler(0, -90 * nextPieceTargetRotationCount + nextPieceTargetRotation.y, 0) * Quaternion.identity;
        return true;
    }
    public bool SetInstance(GameObject nextPiecePrefab)
    {
        if (nextPieceInstance != null)
            Destroy(nextPieceInstance);

        nextPieceInstance = Instantiate(nextPiecePrefab, previewPos.transform);
        nextPieceTargetRotation = nextPieceInstance.transform.rotation;
        nextPieceTargetRotationCount = 0;
        // nextPieceInstance
        return true;
    }

    void LateUpdate()
    {
        nextPieceInstance.transform.rotation = Quaternion.Slerp(nextPieceInstance.transform.rotation, nextPieceTargetRotation, nextPieceTargetRotateSpeed);
    }
}
