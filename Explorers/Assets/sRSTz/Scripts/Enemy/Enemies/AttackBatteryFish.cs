using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBatteryFish : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        aniEvent.OnEnemyAttackEvent += Attack;
        aniEvent.EndEnemyAttackEvent += () => { isAttack = false; moveSpeed = defaultSpeed; };
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

                // 计算弹飞的方向
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // 给玩家一个弹飞的力
                player.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
                player.gameObject.GetComponent<PlayerController>().TakeDamage(damage);

                //Vertigo(-transform.forward * 5f, ForceMode.Impulse, 0.3f);

            }
        }
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
    private void OnCollisionExit(Collision collision)
    {
        touchedCollision = null;
    }


}
