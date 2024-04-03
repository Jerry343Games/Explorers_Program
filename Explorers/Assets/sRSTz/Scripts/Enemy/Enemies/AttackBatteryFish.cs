using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBatteryFish : Enemy
{
    private void FixedUpdate()
    {
        GetClosestPlayer();
        Move();
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            Attack();
        }
    }
    public  void Attack()
    {

        if (touchedCollision != null && canAttack)
        {

            // ���㵯�ɵķ���
            Vector2 direction = (touchedCollision.transform.position - transform.position).normalized;

            // �����һ�����ɵ���
            touchedCollision.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
            touchedCollision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

   

    public void Move()
    {
        if (target!=null&&EnemyManager.Instance.battery != null && canMove&& (target.transform.position - transform.position).magnitude < detectionRange) // ȷ����Ҵ���
        {
            
            Vector2 direction = (EnemyManager.Instance.battery.transform.position - transform.position).normalized; // ��ȡ������ҵĵ�λ����
            rb.velocity = direction * moveSpeed; // ���ų�����ҵķ����ƶ�

            // ������ķ�������Ϊ����õ��ķ���
            EnemyRotate(direction, 15f);
        }
        else if (canMove) //�����ʧ��Ҳ������ƶ�����ô�ص�������
        {
            ReturnSpawnpoint();
        }
    }
    

    
}
