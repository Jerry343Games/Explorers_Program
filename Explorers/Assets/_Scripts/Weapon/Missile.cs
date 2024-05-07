using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float _speed;
    private int _damage;
    public GameObject _target;
    public float _rotateSpeed;
    private Rigidbody _rb;

    public LayerMask enemyLayer;
    public float attackRangeWhenFlying;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //Destroy(gameObject, 1f);
    }

    public void Init(int salveMissileDamage, float salvoMissileSpeed, GameObject target)
    {


        _speed = salvoMissileSpeed;
        _damage = salveMissileDamage;
        _target = target;

        Invoke("Boom", 3f);
    }

    private void FixedUpdate()
    {
        if(_target)
        {
            MoveForward();
            RotateRocket();
        }
        else
        {
            _target = FindNearestEnemy().gameObject;
            if (_target) return;
            Instantiate(Resources.Load<GameObject>("Effect/SmallRocketExplosion"), transform.position, Quaternion.identity);
            MusicManager.Instance.PlaySound("震爆");

            Destroy(gameObject);
        }
    }
    public Collider FindNearestEnemy()
    {
        //找最近的敌人
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRangeWhenFlying, enemyLayer);
        if(colliders.Length==0)
        {
            return null;
        }
        Collider nearest = colliders[0];
        foreach (var coll in colliders)
        {
            if (Vector3.Distance(coll.transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
            {
                if(coll.gameObject.activeInHierarchy)
                {
                    nearest = coll;
                }
            }
        }
        return nearest;
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                Instantiate(Resources.Load<GameObject>("Effect/SmallRocketExplosion"), transform.position, Quaternion.identity);
                other.GetComponent<Enemy>().TakeDamage(_damage);
                MusicManager.Instance.PlaySound("震爆");
                Destroy(gameObject);
                break;
            case "Boss":
                Instantiate(Resources.Load<GameObject>("Effect/SmallRocketExplosion"), transform.position, Quaternion.identity);
                GiantRockCrab.Instance.TakeDamage(_damage);
                MusicManager.Instance.PlaySound("震爆");
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
    private void Boom()
    {
        Instantiate(Resources.Load<GameObject>("Effect/SmallRocketExplosion"), transform.position, Quaternion.identity);
        MusicManager.Instance.PlaySound("震爆");

        Destroy(gameObject);
    }
    private void MoveForward()
    {
        _rb.velocity = transform.up * _speed;
    }

    private void RotateRocket()
    {
        Vector3 targetDirection = (_target.transform.position - transform.position).normalized; // 计算目标方向
        targetDirection.z = 0; // 确保导弹不会离开XY平面

        // 计算新的旋转角度
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        
    }
}
