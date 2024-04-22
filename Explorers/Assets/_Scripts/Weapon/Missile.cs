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
        Destroy(gameObject, 1f);
    }

    public void Init(int salveMissileDamage, float salvoMissileSpeed, GameObject target)
    {


        //GetComponent<BoxCollider>().enabled = false;
        _speed = salvoMissileSpeed;
        _damage = salveMissileDamage;
        _target = target;


        //Vector3 startPos = transform.position;
        //// 计算贝塞尔曲线路径点
        //Vector3 midPos_1 = startPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
        //Vector3 midPos_2 = midPos_1 + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);

        //Vector3[] path = new Vector3[] {
        //    transform.position,
        //    midPos_1,
        //    midPos_2,
        //    new Vector3(target.transform.position.x,target.transform.position.y,0)
        //};

        //// 利用DoTween实现路径动画
        //transform.DOPath(path, 120/_speed, PathType.CatmullRom)
        //    .SetOptions(true)
        //    .OnComplete(() => GetComponent<BoxCollider>().enabled = true); // 动画完成后执行MissileHitTarget方法

        //// 每帧更新导弹的目标位置
        //DOTween.To(() => target.transform.position, x => target.transform.position = x, target.transform.position, 60/_speed).SetOptions(true).SetEase(Ease.Linear);
    }

    private void FixedUpdate()
    {
        
        MoveForward();
        RotateRocket();
        // if (_target)
        //     transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Vector3.Angle(_target.transform.position - transform.position, Vector3.right) * (transform.position.y < _target.transform.position.y ? 1 : -1)));
        // _rb.velocity = (_target.transform.position - transform.position).normalized * _speed;
        
        
        //if (_target)
        //{
        //    // 计算导弹的运动方向
        //    Vector3 direction = transform.position - transform.position;
        //    if (direction != Vector3.zero)
        //    {
        //        Quaternion lookRotation = Quaternion.LookRotation(direction);
        //        transform.rotation = Quaternion.Euler(0f, 0f, lookRotation.eulerAngles.z);
        //    }
        //}
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
