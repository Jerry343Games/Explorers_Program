using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropelAttackArea : MonoBehaviour
{
    public PropelBackpack backpack;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().Vertigo(-collision.transform.right * backpack.attackForce);
            collision.gameObject.GetComponent<Enemy>().TakeDamage(backpack.damage);
        }
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Battery"))
        {
            collision.gameObject.GetComponent<PlayerController>().Vertigo(-collision.transform.right * backpack.attackForce);
            backpack.user.GetComponent<PlayerController>().TakeDamage(backpack.damage);
            backpack.Exit();
        }
    }
}
