using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBatteryFish : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        aniEvent.OnEnemyAttackEvent += Attack;
        aniEvent.EndEnemyAttackEvent += () => { isAttack = false; };
    }
    private void FixedUpdate()
    {
        //GetClosestPlayer();
        if (isSleeping)
        {
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            return;
        }
        if (!isAttack)
            Move();
        else rb.velocity = Vector3.zero;
    }
    
    public  void Attack()
    {


        if (playersInAttackArea.Count == 0) return;
        foreach (var player in playersInAttackArea)
        {
            if (player != null && canAttack)
            {

                // ���㵯�ɵķ���
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // �����һ�����ɵ���
                player.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
                player.gameObject.GetComponent<PlayerController>().TakeDamage(damage);

                //Vertigo(-transform.forward * 5f, ForceMode.Impulse, 0.3f);

            }
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
