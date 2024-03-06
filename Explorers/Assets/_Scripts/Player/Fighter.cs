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
            //�����������������
            case "ReconnectArea":
                if(!_hasConnected /*&& ����������*/)
                {
                    ReconnectRope();
                }
                break;
            //�ռ���������Ʒ
            case "Item":
                other.GetComponent<Item>().Apply(gameObject);
                break;
            default:
                break;
        }
    }
    public void PerformAttack()
    {
        // ��ȡ��ҳ���
        Vector3 playerDirection = transform.right;

        // ���������Լ��ǰ���ĵ���
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
                    // �ڹ�����Χ�ͽǶ��ڵĵ����ܵ�����
                    enemy.GetComponent<Enemy>().TakeDamage(attack); // ���������һ����Ϊ"EnemyHealth"�Ľű��������˺�
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 forwardVector = transform.right;
        Vector3 arcStart = Quaternion.Euler(0, 0, -attackAngle / 2) * forwardVector;
        float stepSize = 5f; // ÿ���߶�֮��ĽǶȼ��

        for (float angle = -attackAngle / 2; angle <= attackAngle / 2; angle += stepSize)
        {
            Vector3 nextPoint = Quaternion.Euler(0, 0, angle) * forwardVector;
            Gizmos.DrawLine(transform.position, transform.position + nextPoint * attackRange);
        }

        // �������һ���߶Σ����ӵ����ε���㣬��������ʾ���η�Χ
        Gizmos.DrawLine(transform.position, transform.position + arcStart * attackRange);
    }
}
