using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    private bool _hasDamaged;
    private Rigidbody _rb;
    [HideInInspector]
    public Transform startPos;
    [HideInInspector]
    public Transform endPos;
    private float _damage;

    public Transform arcStart;
    public Transform arcEnd;

    private GameObject _enemy;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        //同步电弧起点终点
        arcStart.position = startPos.position;
        arcEnd.position = endPos.position;
        if (Vector3.Distance(arcEnd.position,endPos.position)<0.4&&!_hasDamaged)
        {
            _enemy.GetComponent<Enemy>().TakeDamage((int)_damage);
            Instantiate(Resources.Load("Effect/ArcBuleExpolsion"), arcEnd.position, Quaternion.identity);
            _hasDamaged = true;
        }
    }

    public void Init(int damage,GameObject enemy)
    {
        _damage = damage;
        _enemy = enemy;
        arcEnd.position = startPos.position;
        Destroy(gameObject,0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                //伤害执行挪到电池里直接结算了
                break;
            default:
                break;
        }
    }
}
