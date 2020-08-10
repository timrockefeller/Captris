using System.Collections.ObjectModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 防御塔类型
/// </summary>
public enum DefenderType
{
    Default
}

/// <summary>
/// 防御塔
/// </summary>
[RequireComponent(typeof(Collision))]
public class Defender : MonoBehaviour
{
    [Header("References Objects")]
    public GameObject bulletPrefab;
    
    [Header("Attack Properties")]
    [Tooltip("攻击伤害值")]
    public float attackAmount = 10F;
    [Tooltip("攻击CD")]
    public float attackCoolDown = 5F;
    /// <summary>
    /// 当前冷却
    /// </summary>
    private float curCoolDown;
    /// <summary>
    /// 视野内敌人列表
    /// </summary>
    private List<GameObject> aimList;
    
    public DefenderType type = DefenderType.Default;
    private void Awake(){
        transform.localScale = Vector3.zero;
    }
    private void Start()
    {
        aimList = new List<GameObject>();
        curCoolDown = 0;
    }
    private bool fullsize = false;
    private void Update() {
    }
    private void FixedUpdate()
    {
        
        if(!fullsize)transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one,Time.deltaTime*4);
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
            while (!aimList[i].GetComponent<Health>().IsAlive())
            {
                i++;
                if (i >= aimList.Count)
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
