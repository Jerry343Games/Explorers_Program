using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEel : Enemy
{
    private PlayerController reversedPlayer=null;
    public float moveSlowTime = 5f;
    public GameObject playerBeAttackedEffect;
    public GameObject electricEffect;
    public float attackDelay = 3f;
    protected override void Awake()
    {
        base.Awake();
        aniEvent.OnEnemyAttackEvent += Attack;
        aniEvent.EndEnemyAttackEvent += () => { isAttack = false; moveSpeed = defaultSpeed; };
    }
    private void FixedUpdate()
    {
        //GetClosestPlayer();
        if (!isAttack)
            Move();
        else rb.velocity = Vector3.zero;
    }
    
    public  void Attack()
    {
       
        if (playersInAttackArea.Count == 0) return;
       // MusicManager.Instance.PlaySound("����˺ҧ");

        foreach (var player in playersInAttackArea)
        {
            if (player != null && canAttack)
            {
                GameObject effect = Instantiate(electricEffect);
                effect.transform.position = transform.position;
                effect.transform.SetParent(transform);
                // ���㵯�ɵķ���
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // �����һ�����ɵ���
                player.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
                player.gameObject.GetComponent<PlayerController>().TakeDamage(damage);

                //Vertigo(-transform.forward * 5f, ForceMode.Impulse, 0.3f);
                player.GetComponent<PlayerController>().MoveSlow(moveSlowTime);
                GameObject playerEffect = Instantiate(playerBeAttackedEffect);
                playerEffect.transform.position = player.transform.position;
                playerEffect.transform.SetParent(player.transform);
                playerEffect.GetComponent<DestoryByLifeTime>().lifeTime = moveSlowTime;
            }
        }
        MusicManager.Instance.PlaySound("���㹥��");
        canAttack = false;
        Invoke(nameof(RefreshAttack), attackDelay);
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
    public void RefreshAttack()
    {
        canAttack = true;
    }


}
