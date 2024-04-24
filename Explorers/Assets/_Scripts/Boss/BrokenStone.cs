using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenStone : MonoBehaviour
{
    private Rigidbody _rb;

    private int _damage;

    private float _force;

    private float _speed;

    private float _duration;

    private Vector3 _dir;

    public void Init(Vector3 dir,int damage, float force, float speed,float duration)
    {
        _rb = GetComponent<Rigidbody>();
        _dir = dir;
        _damage = damage;
        _force = force;
        _speed = speed;
        _duration = duration;

        Destroy(gameObject, _duration);
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                other.gameObject.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * _force, ForceMode.Impulse);
                break;
            case "Player":
                other.gameObject.GetComponent<PlayerController>().TakeDamage(_damage);
                other.gameObject.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * _force, ForceMode.Impulse);
                break;
            default:
                break;
        }
    }


}
