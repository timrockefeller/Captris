using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float staying = 0;
    private float curStaying = 0;
    public float attenuation;
    void FixedUpdate()
    {
        if (curStaying >= staying || staying == 0)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, attenuation * Time.fixedDeltaTime);
            if (this.transform.localScale.magnitude < 0.02f) Destroy(gameObject);
        }
        else
        {
            curStaying += Time.fixedDeltaTime;
        }
    }
    private bool damageMutex = true;
    public bool attackTerrain = false;
    public bool attackPlayer = false;
    // public bool attackEnemy = false;
    private void OnTriggerEnter(Collider other)
    {
        if (attackPlayer && other.tag == "Player")
        {
            if (damageMutex)
            {
                GameObject.Find("PlayerStatsManager").GetComponent<PlayerStatsManager>().TakeDamage(80);
                damageMutex = false;
            }
        }
        if (attackTerrain && other.tag == "Terrain")
        {
            other.GetComponent<TerrainUnit>().SetEmpty();
        }
        Debug.Log("Explotion triggered a " + other.tag);
    }
}
