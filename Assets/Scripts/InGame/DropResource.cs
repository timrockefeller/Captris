using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropResource : MonoBehaviour
{
    public ResourceType type;

    private PlayManager playManager;

    public Vector3Int position;

    private void Awake()
    {
        this.playManager = GameObject.Find("PlayManager").gameObject.GetComponent<PlayManager>();
        isMoving = false;
    }

    private Vector3 moveTarget;
    private bool isMoving;
    private void Update()
    {
        if (isMoving /*|| can gain*/ && playManager.CanGainResource(type))
        {
            transform.position = transform.position + ( moveTarget - transform.position) / (moveTarget - transform.position).magnitude * Time.deltaTime;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Collector")
        {
            moveTarget = other.transform.position;
            isMoving = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playManager.GainResource(this.type, this.position))
                Destroy(gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Collector")
        {
            isMoving = false;
        }
    }

}
