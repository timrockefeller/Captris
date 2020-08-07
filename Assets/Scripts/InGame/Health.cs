using UnityEngine;
using System.Collections;
public class Health : MonoBehaviour
{
    private PlayManager playManager;
    public float maxHealth = 100;
    public bool IsAlive()
    {
        return curHealth > 0;
    }
    public float curHealth;
    private void Start()
    {
        playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
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
        playManager.SendEvent(PlayEventType.HEALTH_BEATTACKED);

        return true;
    }
    IEnumerator DelayDestroy()
    {
        Debug.Log("Enemy Died");
        yield return new WaitForSeconds(5F);
        Destroy(gameObject);
    }
}
