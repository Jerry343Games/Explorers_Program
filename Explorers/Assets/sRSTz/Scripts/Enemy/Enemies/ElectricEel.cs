using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEel : Enemy
{
    private PlayerController reversedPlayer=null;
    public float reverseTime = 5f;
    protected override void Awake()
    {
        base.Awake();
        aniEvent.OnEnemyAttackEvent += Attack;
        aniEvent.EndEnemyAttackEvent += () => { isAttack = false; };
    }
    private void FixedUpdate()
    {
        //GetClosestPlayer();
        if (!isAttack)
            Move();
        else rb.velocity = Vector3.zero;
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            //Attack();
            animator.Play("Attack");
            Invoke(nameof(Attack), GetAnimatorLength(animator, "Attack")/1.5f);
        }
    }*/
    public  void Attack()
    {
        /*
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
        */
        if (playersInAttackArea.Count == 0) return;
        MusicManager.Instance.PlaySound("����˺ҧ");

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
                player.GetComponent<PlayerController>().MoveReverse(reverseTime);

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
    


}
