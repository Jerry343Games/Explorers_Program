using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDamageCheck : MonoBehaviour
{

    private int _damage;

    private float _force;
    public void Init(int damage,float force)
    {
        _damage = damage;
        _force = force;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Enemy"|| other.gameObject.tag == "Boss")
        {
            if(other.GetComponent<Enemy>())
            {
                other.GetComponent<Enemy>().TakeDamage(_damage);
                other.GetComponent<Rigidbody>().AddForce((other.transform.position-transform.position).normalized*_force,ForceMode.Impulse);
            }
            else
            {
                GiantRockCrab.Instance.TakeDamage(_damage);
            }
        }
    }
}
