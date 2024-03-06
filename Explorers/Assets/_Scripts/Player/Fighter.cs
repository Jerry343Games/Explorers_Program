using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : PlayerController
{
    
    public float attackAngle = 90f;
    public LayerMask enemyLayer;

    
    // Start is called before the first frame update
    void Awake()
    {
        
        PlayerInit();   
    }

    // Update is called once per frame
    void Update()
    {
        if (hasDead) return;
        CharacterMove();
        CheckDistanceToBattery();
        if (playerInputSetting.GetAttackButtonDown())
        {
            PerformAttack();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch(other.tag)
        {
            //进入可重连绳子区域
            case "ReconnectArea":
                if(!_hasConnected /*&& 按下重连键*/)
                {
                    ReconnectRope();
                }
                break;
            //收集到场景物品
            case "Item":
                other.GetComponent<Item>().Apply(gameObject);
                break;
            default:
                break;
        }
    }
    public void PerformAttack()
    {
        // 获取玩家朝向
        Vector3 playerDirection = transform.right;

        // 发射射线以检测前方的敌人
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, playerDirection, attackRange, enemyLayer);

        foreach (RaycastHit2D hit in hits)
        {
            GameObject enemy = hit.collider.gameObject;
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= attackRange)
            {
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(playerDirection, directionToEnemy);

                if (angle < attackAngle / 2)
                {
                    // 在攻击范围和角度内的敌人受到攻击
                    enemy.GetComponent<Enemy>().TakeDamage(attack); // 假设敌人有一个名为"EnemyHealth"的脚本来处理伤害
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 forwardVector = transform.right;
        Vector3 arcStart = Quaternion.Euler(0, 0, -attackAngle / 2) * forwardVector;
        float stepSize = 5f; // 每条线段之间的角度间隔

        for (float angle = -attackAngle / 2; angle <= attackAngle / 2; angle += stepSize)
        {
            Vector3 nextPoint = Quaternion.Euler(0, 0, angle) * forwardVector;
            Gizmos.DrawLine(transform.position, transform.position + nextPoint * attackRange);
        }

        // 绘制最后一条线段，连接到扇形的起点，以完整显示扇形范围
        Gizmos.DrawLine(transform.position, transform.position + arcStart * attackRange);
    }
}
