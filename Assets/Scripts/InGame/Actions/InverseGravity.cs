using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InverseGravity : MonoBehaviour
{
    private void Awake() {
            Physics.gravity = new Vector3(0, -0.1F, 0);
    }
}
