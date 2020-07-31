using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropResource : MonoBehaviour
{
    public ResourceType type;

    private PlayManager playManager;

    public Vector3 position;

    private void Awake()
    {
        this.playManager = GameObject.Find("PlayManager").gameObject.GetComponent<PlayManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playManager.GainResource(this.type))
                Destroy(gameObject);

        }
    }


}
