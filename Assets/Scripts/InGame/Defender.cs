using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DefenderType
{
    Default
}




[RequireComponent(typeof(Collision))]
public class Defender : MonoBehaviour
{
    public DefenderType type = DefenderType.Default;


    private void OnCollisionStay(Collision other)
    {
        if (other.collider.tag == "Enemy")
        {
            // TODO Do Attack

        }
    }
}
