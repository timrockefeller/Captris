using UnityEngine;
using UnityEngine.UI;
public class PlayerStatsManager : MonoBehaviour
{
    public float maxHP = 100F;

    [Tooltip("自动回复延时")]
    public float recoverDelay = 1F;
    public float recoverSpeed = 20F;

    [Header("HUD Components reference")]
    public GameObject healthBar;
    private Image healthBarCMP;
    public GameObject mainCamera;
    private TiltShift mainCameraCMP;

    public float curHP { get; private set; }
    private float recoverWaiting = 0;
    private bool hurting = false;
    private void Start()
    {
        healthBarCMP = healthBar.GetComponent<Image>();
        mainCameraCMP = mainCamera.GetComponent<TiltShift>();
        curHP = maxHP;
    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
        healthBarCMP.fillAmount = curHP / maxHP;
        mainCameraCMP.bloodOutNum = Mathf.Pow(Mathf.Clamp01(1 - curHP / maxHP), 2);
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