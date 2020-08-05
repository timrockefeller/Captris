using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float attenuation;
    void FixedUpdate()
    {
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, attenuation * Time.fixedDeltaTime);
        if (this.transform.localScale.magnitude < 0.02f) Destroy(gameObject);
    }
    private bool damageMutex = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (damageMutex)
            {
                GameObject.Find("PlayerStatsManager").GetComponent<PlayerStatsManager>().TakeDamage(80);
                damageMutex = false;
            }
        }
    }
}
