using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piranha:Enemy
{
    private void FixedUpdate()
    {
        GetClosestPlayer();
        Move();
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            Attack();
        }
    }
    public void Attack()
    {
        
        if (touchedCollision != null)
        {
            
            // ���㵯�ɵķ���
            Vector2 direction = (touchedCollision. transform.position- transform.position).normalized;

            // �����һ�����ɵ���
            touchedCollision.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
            touchedCollision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    public void Move()
    {
        if (target != null&&canMove&& (target.transform.position - transform.position).magnitude < detectionRange)// ȷ����Ҵ���
        {
            
            Vector2 direction = (target.transform.position - transform.position).normalized; // ��ȡ������ҵĵ�λ����
            
            rb.velocity = direction * moveSpeed; // ���ų�����ҵķ����ƶ�

            // ������ķ�������Ϊ����õ��ķ���
            gameObject.transform.right = direction;
        }
        else if(canMove) //�����ʧ��Ҳ������ƶ�����ô�ص�������
        {
            ReturnSpawnpoint();
        }
    }

    
    
  
    

    
}
