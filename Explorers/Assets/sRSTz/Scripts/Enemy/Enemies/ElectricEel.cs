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
       // MusicManager.Instance.PlaySound("怪物撕咬");

        foreach (var player in playersInAttackArea)
        {
            if (player != null && canAttack)
            {
                GameObject effect = Instantiate(electricEffect);
                effect.transform.position = transform.position;
                effect.transform.SetParent(transform);
                // 计算弹飞的方向
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // 给玩家一个弹飞的力
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
        MusicManager.Instance.PlaySound("电鱼攻击");
        canAttack = false;
        Invoke(nameof(RefreshAttack), attackDelay);
    }

    public void Move()
    {
        if (canMove)// 确保玩家存在
        {

            //Vector2 direction = (target.transform.position - transform.position).normalized; // 获取朝向玩家的单位向量

            rb.velocity = enemyAI.FinalMovement * moveSpeed; // 沿着影响因子计算出的方向移动

            // 将人物的方向设置为计算得到的方向
            //gameObject.transform.right = direction;
            EnemyRotate();
        }
        /*
        else if(canMove) //如果丢失玩家并且能移动，那么回到出生点
        {
            ReturnSpawnpoint();
        }*/
    }
    public void RefreshAttack()
    {
        canAttack = true;
    }


}
