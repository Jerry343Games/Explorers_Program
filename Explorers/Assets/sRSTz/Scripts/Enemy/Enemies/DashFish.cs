using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DashFishData", menuName = "EnemyData/DashFishData", order = 2)]
public class DashFish : EnemySO
{
    public float detectionRadius = 5f; // 检测半径
    private bool isDashing=false;
    public float prepareTime = 2f;
    private float prepareTimer = 0;
    public float dashTime = 3f;
    private float dashTimer = 0;
    public float dashForce = 3f;
    public override void Attack(Enemy enemy)
    {
        if (enemy.touchedCollision != null)
        {
            // 计算弹飞的方向
            Vector2 direction = (enemy.touchedCollision.transform.position - enemy.transform.position).normalized;

            // 给玩家一个弹飞的力
            enemy.touchedCollision.gameObject.GetComponent<Rigidbody>().AddForce(direction * enemy.force, ForceMode.Impulse);
        }
    }

    public override void FixUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void Move(Enemy enemy)
    {
        if (enemy.target == null) return; // 确保玩家存在
        Vector2 distance = (enemy.target.transform.position - enemy.transform.position);
            Vector2 direction =distance. normalized; // 获取朝向玩家的单位向量
        if (prepareTimer==0&& Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) >= Mathf.Pow(detectionRadius, 2)){
            enemy.rb.velocity = direction * enemy.moveSpeed; // 沿着朝向玩家的方向移动

            // 将人物的方向设置为计算得到的方向
            enemy.gameObject.transform.right = direction;
        }
        else
        {
            // 将人物的方向设置为计算得到的方向
            enemy.gameObject.transform.right = direction;
            //如果正在冲刺
            if (isDashing)
            {
                if (dashTimer < dashTime)
                {
                    dashTimer += Time.deltaTime;
                }
                else
                {
                    dashTimer = 0;
                    isDashing = false;
                    enemy.rb.velocity = new Vector3(0, 0, 0);
                }
            }
            else if(!isDashing)//如果还没冲刺
            {
                if (prepareTimer < prepareTime)
                {
                    prepareTimer += Time.deltaTime;
                }
                else
                {
                    prepareTimer = 0;
                    isDashing = true;
                    enemy.rb.AddForce(direction * dashForce, ForceMode.Impulse);
                }


                

            }
            





        }
            
        


    }

    public override void TakeDamage()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
    


}
