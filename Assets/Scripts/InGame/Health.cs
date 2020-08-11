using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Health : MonoBehaviour
{
    public GameObject healthBarUIPrefab;
    private GameObject healthBarUIInstance;
    private Image healthBarUIFill;
    private Image healthBarUIBack;
    private float targetAlpha = 0;
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
        healthBarUIInstance = Instantiate(healthBarUIPrefab, transform.position, Quaternion.identity);
        healthBarUIInstance.transform.SetParent(null);
        healthBarUIInstance.transform.position += Vector3.up * 0.5f;
        healthBarUIFill = healthBarUIInstance.transform.Find("fill").GetComponent<Image>();
        healthBarUIBack = healthBarUIInstance.transform.Find("back").GetComponent<Image>();
    }

    private void Update()
    {
        healthBarUIInstance.transform.position = Vector3.Lerp(healthBarUIInstance.transform.position, transform.position, 2 * Time.deltaTime);
        if (healthBarUIFill != null && healthBarUIBack != null)
        {
            healthBarUIFill.fillAmount = Mathf.Lerp(healthBarUIFill.fillAmount, curHealth / maxHealth, 2 * Time.deltaTime);
            healthBarUIFill.color = new Color(1F, 1F, 1F, Mathf.Clamp01(targetAlpha));
            healthBarUIBack.color = new Color(0F, 0F, 0F, Mathf.Clamp01(targetAlpha) / 2F);
        }
    }
    private void FixedUpdate()
    {
        targetAlpha -= Time.deltaTime * 2;
    }

    /// <summary>
    /// 造成伤害
    /// </summary>
    /// <param name="damage">负数为加血</param>
    /// <returns>是否有改变死亡状态</returns>
    public bool DoAttack(float damage, bool needDestroy = true)
    {
        targetAlpha = 5;
        if (curHealth <= 0) return true;
        curHealth -= damage;
        if (curHealth <= 0)
        {
            if (needDestroy)
                StartCoroutine(DelayDestroy());
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
