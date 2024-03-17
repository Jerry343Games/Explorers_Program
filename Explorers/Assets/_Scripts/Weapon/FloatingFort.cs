using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingFort : MonoBehaviour
{

    private Rigidbody _rb;

    private float _damage;
    private float _speed;
    private WeaponDataSO _data;
    public LayerMask enemyLayer;
    private float attackTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(WeaponDataSO data)
    {
        _data = data;
        _damage = data.attackDamage;
        _speed = data.attackSpeed;

        attackTimer = 0;
    }

    private void Update()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _data.attackRange, enemyLayer);
        if(colls.Length==0)
        {
            return;
        }
        if(attackTimer<0)
        {
            attackTimer = _data.attackCD;
            int randomIndex = Random.Range(0, colls.Length);
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().Init(_data, (colls[randomIndex].transform.position - transform.position).normalized);
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }


}
