using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkfish : Enemy
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
        //*************************************************
        if (isSleeping || !canMove)
        {
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            return;
        }
        Move();
        
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            animator.Play("Attack");
            Invoke(nameof(Attack), GetAnimatorLength(animator, "Attack") / 1.5f);
        }
    }*/
    
    public void Attack()
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
    public override void StartledFromSleep()
    {
        base.StartledFromSleep();
        // 


    }
    //public bool isSpawning = false;
    public void SpawnEnemy()
    {

        EnemyManager.Instance.SpawnEnemyAfter();
        // animator.Play("Spawn");
    }

    public Collider[] colliders;
    public float flashRadius;
    
    public void Flash()
    {      
        canMove = false;
        colliders= Physics.OverlapSphere(transform.position, flashRadius);
        foreach(var obj in colliders)
        {
            if (obj.CompareTag("Player") || obj.CompareTag("Battery"))
            {
                // ���ϼ���
            }
            else if (obj.CompareTag("Enemy"))
            {
                //����ֱ�ӻ�Ѫ20
            }
        }
        //animator.Play("Flash");
    }
    public void FlashEnd()
    {
        canMove = true;
    }
    
}
