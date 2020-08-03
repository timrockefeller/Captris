using UnityEngine;
using UnityEngine.UI;
public class PlayerStatsManager : MonoBehaviour
{
    public float maxHP = 100F;

    [Tooltip("自动回复延时")]
    public float recoverDelay = 1F;
    public float recoverSpeed = 20F;

    // HUD Components reference
    public GameObject healthBar;
    private  Image healthBarCMP;

    public float curHP { get; private set; }
    private float recoverWaiting = 0;
    private bool hurting = false;
    private void Start() {
        healthBarCMP = healthBar.GetComponent<Image>();
        curHP = maxHP;
    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
        healthBarCMP.fillAmount = curHP/ maxHP;
        if (curHP < maxHP && !hurting)
        {
            curHP += recoverSpeed * Time.fixedDeltaTime;

        }
        if (hurting)
        {
            recoverWaiting += Time.fixedDeltaTime;
            if (recoverWaiting > recoverDelay)
            {
                hurting = false;
            }
        }
    }
    public void TakeDamage(float hurt)
    {
        curHP -= hurt;
        if (curHP <= 0)
        {
            // TODO GameOver
            
        }
        hurting = true;
        recoverWaiting = 0;
    }
}