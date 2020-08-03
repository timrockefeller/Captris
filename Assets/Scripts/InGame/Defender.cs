using System.Collections.ObjectModel;
using System.Linq;
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
    [Tooltip("伤害值")]
    public float attackAmount = 10F;
    [Tooltip("CD")]
    public float attackCoolDown = 5F;
    private float curCoolDown;
    private List<GameObject> aimList;

    public DefenderType type = DefenderType.Default;
    public GameObject bulletPrefab;
    private void Start()
    {
        aimList = new List<GameObject>();
        curCoolDown = 0;
    }

    private void FixedUpdate()
    {
        if (curCoolDown < attackCoolDown)
        {
            curCoolDown += Time.fixedDeltaTime;

        }
        else
        {
            if (DoAttack())
                curCoolDown -= attackCoolDown;

        }
    }

    bool DoAttack()
    {
        aimList.RemoveAll((g) => g == null);
        if (aimList.Count > 0)
        {
            aimList.OrderBy((g) => (g.transform.position - transform.position).magnitude);
            int i = 0; bool validTarget = true;
            while (!aimList[i].GetComponent<Health>().Alive())
            {
                i++;
                if (i > aimList.Count)
                {
                    validTarget = false;
                    break;

                }
            }
            if (validTarget)
            {
                GameObject instance = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
                instance.GetComponent<FollowAndDamage>().SetTarget(aimList[i], 10F);
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {

        // Debug.Log(other.name);
        if (other.tag == "Enemy")
        {
            aimList.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            aimList.Remove(other.gameObject);
        }
    }

}
