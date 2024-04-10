using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Torpedoes : MonoBehaviour
{
    private LayerMask _enemyLayer;
    private LayerMask _playerLayer;
    private float _speed;
    private float _destoryTime;
    private float _range;
    private float _force;
    private int _damage;
    private Vector3 _dir;
    private float timer;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void Init(LayerMask enemyLayer,LayerMask playerLayer,float speed,float destoryTime,float range,float force,int damage,Vector3 dir)
    {
        _enemyLayer = enemyLayer;
        _playerLayer = playerLayer;
        _speed = speed;
        _destoryTime = destoryTime;
        _range = range;
        _force = force;
        _damage = damage;
        _dir = dir;
        timer = 0;
    }

    private void Update()
    {
        if(timer<_destoryTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            //±¬Õ¨ÌØÐ§
            Instantiate(Resources.Load<GameObject>("Effect/RocketExplosion"),transform.position,Quaternion.identity);
            Collider[] enemyColls = Physics.OverlapSphere(transform.position, _range,_enemyLayer);
            Collider[] playerColls = Physics.OverlapSphere(transform.position, _range, _playerLayer);
            foreach(var coll in enemyColls)
            {
                coll.GetComponent<Enemy>().TakeDamage(_damage);
                coll.GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle * _force, ForceMode.Impulse) ;
            }
            foreach(var coll in playerColls)
            {
                coll.GetComponent<PlayerController>().TakeDamage(_damage);
                coll.GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle * _force * 0.01f, ForceMode.Impulse);
            }
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Enemy" || other.tag == "Player")
        {
            Instantiate(Resources.Load<GameObject>("Effect/RocketExplosion"),transform.position,Quaternion.identity);
            Collider[] enemyColls = Physics.OverlapSphere(transform.position, _range, _enemyLayer);
            Collider[] playerColls = Physics.OverlapSphere(transform.position, _range, _playerLayer);
            foreach (var coll in enemyColls)
            {
                coll.GetComponent<Enemy>().TakeDamage(_damage);
                //coll.GetComponent<Rigidbody>().AddExplosionForce(_force, transform.position, _range);
                coll.GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle * _force, ForceMode.Impulse);
            }
            foreach (var coll in playerColls)
            {
                coll.GetComponent<PlayerController>().TakeDamage(_damage);
                coll.GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle * _force *0.01f, ForceMode.Impulse);
            }
            Destroy(gameObject);
        }
    }
}
