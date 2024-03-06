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
            
            // 计算弹飞的方向
            Vector2 direction = (touchedCollision. transform.position- transform.position).normalized;

            // 给玩家一个弹飞的力
            touchedCollision.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
            touchedCollision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    public void Move()
    {
        if (target != null) // 确保玩家存在
        {
            Vector2 direction = (target.transform.position - transform.position).normalized; // 获取朝向玩家的单位向量
            rb.velocity = direction * moveSpeed; // 沿着朝向玩家的方向移动

            // 将人物的方向设置为计算得到的方向
            gameObject.transform.right = direction;
        }
    }

    public  void TakeDamage()
    {
        
    }

  
    

    
}
