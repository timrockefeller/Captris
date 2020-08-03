using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAndDamage : MonoBehaviour
{

    public string targetTag = "Enemy";
    public GameObject target = null;
    public float damage = 10F;
    public float speed = 2F;
    public float torque = 5F;
    private float targetDelay = 0.5f;
    private float _tDelay;
    private float _passDelay;
    private void FixedUpdate()
    {
        if (target != null)
        {
            _tDelay = 0;
            transform.position = transform.position + transform.forward * speed * Time.fixedDeltaTime + _passDelay * Time.fixedDeltaTime * (target.transform.position - transform.position);
            Quaternion TargetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.fixedDeltaTime * torque);
            torque += 5 * Time.fixedDeltaTime;
            _passDelay += 2 * Time.fixedDeltaTime;
        }
        else
        {
            _tDelay += Time.fixedDeltaTime;
            if (_tDelay > targetDelay)
            {
                this.GetComponent<MeshFilter>().mesh = null;
                StartCoroutine("DelayDestroy");
            }
        }
    }
    private void Start()
    {
        Vector3 face = new Vector3(RD.NextFloat() - 0.5f, RD.NextFloat(), RD.NextFloat() - 0.5f).normalized;
        this.transform.rotation = Quaternion.FromToRotation(Vector3.zero, face);
        _passDelay = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (target != null)
        {
            if (other.tag == targetTag)
            {
                other.GetComponent<Health>().DoAttack(damage);
                this.GetComponent<MeshFilter>().mesh = null;
                target = null;
                StartCoroutine("DelayDestroy");
            }
        }
    }
    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(5F);
        Destroy(gameObject);
    }

    public void SetTarget(GameObject t, float d = 10F)
    {
        this.target = t;
        this.damage = d;
    }
}
