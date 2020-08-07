using UnityEngine;
using System.Collections;
public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    public bool IsAlive()
    {
        return curHealth > 0;
    }
    public float curHealth;
    private void Start()
    {
        curHealth = maxHealth;
    }
    /// <summary>
    /// 造成伤害
    /// </summary>
    /// <param name="damage">负数为加血</param>
    /// <returns>是否死亡</returns>
    public bool DoAttack(float damage)
    {
        if (curHealth <= 0) return true;
        curHealth -= damage;
        if (curHealth <= 0)
        {
            StartCoroutine("DelayDestroy");
            return false;
        }
        curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
        return true;
    }
    IEnumerator DelayDestroy()
    {
        Debug.Log("Enemy Died");
        yield return new WaitForSeconds(5F);
        Destroy(gameObject);
    }
}
