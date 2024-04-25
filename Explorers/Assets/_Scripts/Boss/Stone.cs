using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private Rigidbody _rb;

    private Vector3 _dir;

    private float _speed;

    private int _flyingDamage;

    private float _force;

    private float _duration;

    private int _boomDamage;

    private float _boomRange;

    private float flyingTimer;

    public LayerMask playerLayer;

    public void Init(Vector3 dir,float speed,int flyingDamage,float force,float duration,int boomDamage,float boomRange)
    {
        _rb = GetComponent<Rigidbody>();

        _dir = dir;
        _speed = speed;
        _flyingDamage = flyingDamage;
        _force = force;
        _duration = duration;
        _boomDamage = boomDamage;
        _boomRange = boomRange;

        flyingTimer = _duration;
    }

    private void Update()
    {
        if (flyingTimer > 0)
        {
            flyingTimer -= Time.deltaTime;
        }
        else
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, _boomRange, playerLayer);

            foreach(var coll in colls)
            {
                if(coll.gameObject.tag=="Player")
                {
                    coll.gameObject.GetComponent<PlayerController>().TakeDamage(_boomDamage);
                    coll.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - coll.transform.position).normalized * _force, ForceMode.Impulse);

                }
                else if(coll.gameObject.tag == "Enemy")
                {
                    coll.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - coll.transform.position).normalized * _force, ForceMode.Impulse);
                }
            }
            //生成特效

            //销毁
            Destroy(gameObject);


        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (flyingTimer <= 0) return;
        switch(other.gameObject.tag)
        {
            case "Enemy":
                other.gameObject.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * _force, ForceMode.Impulse);
                break;
            case "Player":
                other.gameObject.GetComponent<PlayerController>().TakeDamage(_flyingDamage);
                other.gameObject.GetComponent<Rigidbody>().AddForce((other.transform.position-transform.position).normalized * _force, ForceMode.Impulse);
                break;
            default:
                break;
        }
    }
}

