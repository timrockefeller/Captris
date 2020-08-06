using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    public GameObject globalMask;
    private Image globalMaskCMP;
    private float globalMaskAlpha = 0;

    // [Header("Parameters")]
    public float curHP { get; private set; }
    private float recoverWaiting = 0;
    private bool hurting = false;
    private Camera mainCameraRc;
    private void Start()
    {
        healthBarCMP = healthBar.GetComponent<Image>();
        mainCameraCMP = mainCamera.GetComponent<TiltShift>();
        mainCameraRc = mainCamera.GetComponent<Camera>();
        globalMaskCMP = globalMask.GetComponent<Image>();
        curHP = maxHP;
        globalMaskAlpha = 0;
        Time.timeScale = 1;
    }

    public bool IsAlive()
    {
        return curHP > 0;
    }
    public bool deathflag = false;
    private void Update()
    {
        if (Input.GetKey(KeyCode.I))
        {
            curHP = 0;
        }
        if (!IsAlive())
        {
            deathflag = true;
        }
        if (deathflag)
        {
            mainCameraRc.orthographicSize = Mathf.Lerp(mainCameraRc.orthographicSize, 10, 4 * Time.deltaTime);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.01f, 2 * Time.deltaTime);
            StartCoroutine(GameOver());
        }
        globalMaskCMP.color = Color.Lerp(globalMaskCMP.color, new Color(0, 0, 0, globalMaskAlpha), 20f * Time.deltaTime);
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSecondsRealtime(2);
        // TODO to another scene
        globalMaskAlpha = 1;
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene("Title");
    }
    private void FixedUpdate()
    {
        healthBarCMP.fillAmount = curHP / maxHP;
        if (!deathflag)
            mainCameraCMP.bloodOutNum = Mathf.Pow(Mathf.Clamp01(1 - curHP / maxHP), 2) * 0.8f;
        else mainCameraCMP.bloodOutNum = 1;
        if (!deathflag && curHP < maxHP && !hurting)
        {
            curHP += recoverSpeed * Time.fixedDeltaTime;

        }
        if (!deathflag && hurting)
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
