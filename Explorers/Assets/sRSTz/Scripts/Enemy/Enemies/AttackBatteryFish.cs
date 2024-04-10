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

            // 计算弹飞的方向
            Vector2 direction = (touchedCollision.transform.position - transform.position).normalized;

            // 给玩家一个弹飞的力
            touchedCollision.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
            touchedCollision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
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
