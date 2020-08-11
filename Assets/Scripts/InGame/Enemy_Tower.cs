using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tower : MonoBehaviour
{
    private GameObject player;
    [Header("Attack Porperties")]
    public GameObject bulletPrefab;
    public GameObject explotionPrefab;
    public GameObject giantPrefab;
    public float attackCD;
    private float currentCD;

    private Enemy_Tower_Eye eye;
    private Health health;
    private PlayManager playManager;

    // Start is called before the first frame update
    void Start()
    {
        playManager = GameObject.Find("PlayManager").GetComponent<PlayManager>();
        health = GetComponent<Health>();
        eye = transform.Find("Eye").GetComponent<Enemy_Tower_Eye>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    bool deathFlag = false;
    float deathTimePassed = 0;
    private void FixedUpdate()
    {
        if (health.IsAlive())
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectsWithTag("Player")[0];
                return;
            }


            if (HaveTarget())
                currentCD += Time.fixedDeltaTime;
            if (currentCD > attackCD)
            {
                // do attack
                if (DoAttack()) currentCD = 0;
            }
        }
        else
        {
            if (!deathFlag)
            {
                playManager.SendEvent(PlayEventType.PLAYER_KILL);
                playManager.SendEvent(PlayEventType.PLAYER_KILL_TOWER);
                StartCoroutine(DiedAnim());
                deathTimePassed = -0.2f;
                deathFlag = true;
            }
            deathTimePassed += Time.fixedDeltaTime * 0.4f;
            // transform.parent.rotation = Quaternion.Lerp(dethRotation, Quaternion.LookRotation(Vector3.down, Vector3.forward), (deathTimePassed / 2).Sigmoid());
            transform.parent.position = transform.parent.position + Vector3.down * Mathf.Min(Mathf.Max(0, deathTimePassed), 20f) * Time.fixedDeltaTime;
        }
    }
    IEnumerator DiedAnim()
    {
        GameObject.Find("CamPos").GetComponent<CameraController>().DoVibrate(transform.position);
        // do dieing ~~
        int num = 10;
        while (num-- > 0)
        {
            GameObject instance = Instantiate(explotionPrefab, this.transform);
            Vector3 deltaPos = RD.NextPositionf(1, 1, 1) - Vector3.one * 0.5f;
            instance.transform.position = instance.transform.position + deltaPos;
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1);
        // spawn Giant
        GameObject giant = Instantiate(giantPrefab, new Vector3(transform.position.x, 5, transform.position.z), Quaternion.identity);
        yield return new WaitForSeconds(6);
        Destroy(transform.parent.gameObject);
    }

    private bool previteHaveTarget = false;
    private int previteHaveTargetCount = 0;
    /// <summary>
    /// cd count if has target in field
    /// </summary>
    /// <returns></returns>
    bool HaveTarget()
    {
        previteHaveTargetCount++;
        if (previteHaveTargetCount > 5)
        {
            foreach (var item in eye.aimList)
            {
                if (TerrainUnit.IsManualType(item.GetComponent<TerrainUnit>().type))
                {
                    previteHaveTarget = true;
                    previteHaveTargetCount = 0;
                    return true;
                }
            }
            
                    previteHaveTargetCount = 0;
            previteHaveTarget = false;
            return false;
        }
        return previteHaveTarget;
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
