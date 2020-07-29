using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float attenuation;
    void Update()
    {
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, attenuation * Time.deltaTime);
        if (this.transform.localScale.magnitude < 0.02f) Destroy(gameObject);
    }
}
