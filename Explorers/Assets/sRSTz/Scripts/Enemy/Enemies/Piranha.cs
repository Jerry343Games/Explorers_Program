using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PiranhaData",menuName ="EnemyData/PiranhaData",order =1)]
public class Piranha : EnemySO
{

    public override void Attack(Enemy enemy)
    {
        
        if (enemy.touchedCollision != null)
        {
            
            // 计算弹飞的方向
            Vector2 direction = (enemy.touchedCollision. transform.position- enemy.transform.position).normalized;

            // 给玩家一个弹飞的力
            enemy.touchedCollision.gameObject.GetComponent<Rigidbody>().AddForce(direction * enemy.force, ForceMode.Impulse);
        }
    }

    public override void Move(Enemy enemy)
    {
        if (enemy.target != null) // 确保玩家存在
        {
            Vector2 direction = (enemy.target.transform.position - enemy.transform.position).normalized; // 获取朝向玩家的单位向量
            enemy.rb.velocity = direction * enemy.moveSpeed; // 沿着朝向玩家的方向移动

            // 将人物的方向设置为计算得到的方向
            enemy.gameObject.transform.right = direction;
        }
    }

    public override void TakeDamage()
    {
        
    }

    public override void Update()
    {
        
    }
    

    public override void FixUpdate()
    {
        throw new System.NotImplementedException();
    }
}
