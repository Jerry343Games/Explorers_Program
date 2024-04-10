using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBatteryFish : Enemy
{
    private void FixedUpdate()
    {
        //GetClosestPlayer();
        Move();
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            //Attack();
            //if (enemyAI.GetCurrentTarget() == null) return;
            animator.Play("Attack");
            Invoke(nameof(Attack), GetAnimatorLength(animator, "Attack")/2);
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
        if (canMove)// ȷ����Ҵ���
        {

            //Vector2 direction = (target.transform.position - transform.position).normalized; // ��ȡ������ҵĵ�λ����

            rb.velocity = enemyAI.FinalMovement * moveSpeed; // ����Ӱ�����Ӽ�����ķ����ƶ�

            // ������ķ�������Ϊ����õ��ķ���
            //gameObject.transform.right = direction;
            EnemyRotate();
        }
        /*
        else if(canMove) //�����ʧ��Ҳ������ƶ�����ô�ص�������
        {
            ReturnSpawnpoint();
        }*/
    }
    private void OnCollisionExit(Collision collision)
    {
        touchedCollision = null;
    }


}
