using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropelAttackArea : MonoBehaviour
{
    public PropelBackpack backpack;
    private void Awake()
    {
        // ����ִ����ײ���
        Physics.SyncTransforms();
        Physics.Simulate(Time.fixedDeltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // ���㵯�ɵķ���
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            collision.gameObject.GetComponent<Enemy>().Vertigo(direction * backpack.attackForce);
            collision.gameObject.GetComponent<Enemy>().TakeDamage(backpack.damage);
        }
        //if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Battery"))
        //{
        //    // ���㵯�ɵķ���
        //    Vector2 direction = (collision.transform.position - transform.position).normalized;
        //    collision.gameObject.GetComponent<PlayerController>().Vertigo(direction * backpack.attackForce);
        //    backpack.user.GetComponent<PlayerController>().TakeDamage(backpack.damage);
        //    backpack.Exit();
        //}
    }
}
