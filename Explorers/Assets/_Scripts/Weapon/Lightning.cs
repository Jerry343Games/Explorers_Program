using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    private Rigidbody _rb;

    private float _damage;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(int damage)
    {
        _damage = damage;
        Destroy(gameObject,0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                other.GetComponent<Enemy>().TakeDamage((int)_damage);
                Destroy(gameObject,0.3f);
                break;
            default:
                break;
        }
    }
}
