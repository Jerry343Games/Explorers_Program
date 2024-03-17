using DG.Tweening;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private float _speed;
    private int _damage;
    private GameObject _target;
    private Rigidbody _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 1f);
    }

    public void Init(int salveMissileDamage, float salvoMissileSpeed, GameObject target)
    {
        _speed = salvoMissileSpeed;
        _damage = salveMissileDamage;
        _target = target;
    }

    private void FixedUpdate()
    {
        if(_target)
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Vector3.Angle(_target.transform.position - transform.position, Vector3.right) * (transform.position.y <_target.transform.position.y?1:-1)));
        _rb.velocity = (_target.transform.position - transform.position).normalized * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
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
