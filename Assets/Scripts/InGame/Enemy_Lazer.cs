using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lazer : MonoBehaviour
{

    public GameObject player;

[Header("Motivate Properties")]
    public float maxSpeed;
    public float maxAcc;
    public float maxTorque;

    private float speed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
