using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingFort : MonoBehaviour
{

    private Rigidbody _rb;

    private float _damage;
    private float _speed;
    private WeaponDataSO _data;
    public LayerMask enemyLayer;
    private float attackTimer;
    private Transform _followPoint;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(WeaponDataSO data, Transform point)
    {
        _data = data;
        _damage = data.attackDamage;
        _speed = data.attackSpeed;
        _followPoint = point;
        attackTimer = 0;
    }

    private void Update()
    {
        


        transform.position = Vector3.Lerp(transform.position, _followPoint.position, 0.9f);

        Collider[] colls = Physics.OverlapSphere(transform.position, _data.attackRange, enemyLayer);
        if(colls.Length==0)
        {
            return;
        }
        int randomIndex = Random.Range(0, colls.Length);
        if (attackTimer<0)
        {
            attackTimer = _data.attackCD;
            GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().Init(_data, (colls[randomIndex].transform.position - transform.position).normalized);

            MusicManager.Instance.PlaySound("浮游炮台射击");

        }
        else
        {
            attackTimer -= Time.deltaTime;
            float angle = Vector3.Angle(Vector3.left, (colls[randomIndex].transform.position - transform.position).normalized);
            //旋转方向
            if (colls[randomIndex].transform.position.y < transform.position.y)
            {
                transform.DORotate(new Vector3(0, 0, angle),0.5f);
            }
            else
            {
                transform.DORotate(new Vector3(0, 0, -angle), 0.5f);
            }
        }
    }


}
