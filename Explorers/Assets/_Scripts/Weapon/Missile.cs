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

        Invoke("Boom", 2f);
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
            Instantiate(Resources.Load<GameObject>("Effect/SmallRocketExplosion"), transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                Instantiate(Resources.Load<GameObject>("Effect/SmallRocketExplosion"), transform.position, Quaternion.identity);
                other.GetComponent<Enemy>().TakeDamage(_damage);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
    private void Boom()
    {
        Instantiate(Resources.Load<GameObject>("Effect/SmallRocketExplosion"), transform.position, Quaternion.identity);
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
