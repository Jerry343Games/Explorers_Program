using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Rigidbody _rb;
    private BoxCollider _coll;

    private float _damage;
    private Vector3 _dir;
    private float _speed;
    private float _destroyTime;
    private float _destroyTimer;

    private bool hasInit = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<BoxCollider>();
        _coll.enabled = false;
    }

    public void Init(WeaponDataSO data,Vector3 dir)
    {
        _damage = data.attackDamage;
        _speed = data.attackSpeed;
        _destroyTime = data.attackRange / 60f;
        _destroyTimer = _destroyTime;
        _dir = dir;

        hasInit = true;
    }

    private void Update()
    {
        if (!hasInit) return;
        if (_destroyTimer < 0)
        {
            _coll.enabled = true;
            Destroy(gameObject, 0.3f);
        }
        else
        {
            _destroyTimer -= Time.deltaTime;
        }
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
                other.GetComponent<Enemy>().TakeDamage((int)_damage);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
