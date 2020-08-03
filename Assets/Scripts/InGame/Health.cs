using UnityEngine;
using System.Collections;
public class Health : MonoBehaviour
{
    public float maxHealth = 100;
    public bool Alive()
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
        yield return new WaitForSeconds(5F);
        Destroy(gameObject);
    }
}