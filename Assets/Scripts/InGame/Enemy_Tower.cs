using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tower : MonoBehaviour
{
    private GameObject player;
    [Header("Attack Porperties")]
    public GameObject bulletPrefab;
    public float attackCD;
    private float currentCD;

    private Enemy_Tower_Eye eye;

    // Start is called before the first frame update
    void Start()
    {
        eye = transform.Find("Eye").GetComponent<Enemy_Tower_Eye>();
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
            if (DoAttack()) currentCD = 0;
        }
    }
    bool DoAttack()
    {
        eye.aimList.RemoveAll((g) => g == null || TerrainUnit.IsStaticType(g.type));
        if (eye.aimList.Count > 0)
        {
            eye.aimList.OrderBy((g) => (g.transform.position - transform.position).magnitude);
            int i = 0; bool validTarget = true;
            while (!TerrainUnit.IsManualType(eye.aimList[i].GetComponent<TerrainUnit>().type))
            {
                i++;
                if (i >= eye.aimList.Count)
                {
                    validTarget = false;
                    break;
                }
            }
            if (validTarget)
            {
                GameObject instance = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
                instance.GetComponent<BulletMotivation>().target = eye.aimList[i].gameObject;
                return true;
            }
        }
        return false;

        // return false;
    }
}
