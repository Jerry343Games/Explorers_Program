using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranquilizerBullet : MonoBehaviour
{
    private Rigidbody _rb;

    private float _damage;
    private Vector3 _dir;
    private float _speed;
    private float _destoryTime;
    private float _tranquilizerEffectTime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(WeaponDataSO data, Vector3 dir,float time)
    {
        _damage = data.attackDamage;
        _speed = data.attackSpeed;
        _destoryTime = data.attackRange / data.attackSpeed;
        _dir = dir;
        _tranquilizerEffectTime = time;
        Destroy(gameObject, _destoryTime);//根据射程计算
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                other.GetComponent<Enemy>().Paralysis(_tranquilizerEffectTime);
                other.GetComponent<Enemy>().TakeDamage((int)_damage);
                Instantiate(Resources.Load<GameObject>("Effect/AnaesthesiaBulletExplosion"), transform.position, Quaternion.identity);
                Destroy(gameObject);
                break;
            case "Barrier":
                Instantiate(Resources.Load<GameObject>("Effect/AnaesthesiaBulletExplosion"), transform.position, Quaternion.identity);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
