using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEel : Enemy
{
    private PlayerController reversedPlayer=null;
    public float reverseTime = 5f;
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
            Attack();
        }
    }
    public void Attack()
    {

        if (touchedCollision != null && canAttack)
        {

            // ���㵯�ɵķ���
            Vector2 direction = (touchedCollision.transform.position - transform.position).normalized;
            reversedPlayer = touchedCollision.gameObject.GetComponent<PlayerController>();
            // �����һ�����ɵ���
            reversedPlayer.Vertigo(direction * force);
            reversedPlayer.TakeDamage(damage);

            Vertigo(-transform.forward * 5f, ForceMode.Impulse, 0.3f);
            reversedPlayer.MoveReverse(reverseTime);

            
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
    


}
