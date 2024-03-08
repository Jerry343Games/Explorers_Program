using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Rigidbody _rb;

    private int _damage;
    private Vector3 _dir;
    private float _speed;
    private float _destoryTime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(WeaponDataSO data,Vector3 dir)
    {
        _damage = data.attackDamage;
        _speed = data.attackSpeed;
        _destoryTime = data.attackRange / 60f;
        _dir = dir;

        Destroy(gameObject, _destoryTime);//根据射程计算
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Enemy":
                other.GetComponent<Enemy>().TakeDamage(_damage);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
