using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tower : MonoBehaviour
{
    private GameObject player;
    [Header("Attack Porperties")]

    public float attackCD;
    private float currentCD;

    private GameObject _eye;

    // Start is called before the first frame update
    void Start()
    {
        _eye = transform.Find("Eye").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectsWithTag("Player")[0];
            return;
        }


        currentCD += Time.fixedDeltaTime;
        if (currentCD > attackCD)
        {
            // do attack

        }
    }
}
