using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISamplesV2 : MonoBehaviour
{
    public Transform player; // 玩家的位置
    public float moveSpeed = 2f; // 移动速度
    public float avoidanceStrength = 5f; // 避障的强度
    public LayerMask obstacleMask; // 避障检测的LayerMask
    public float avoidanceRadius = 0.5f; // 避障检测的半径
    public float separationStrength = 3f; // 与同伴分离的力量
    public float separationRadius = 0.3f; // 与同伴分离的半径
    public LayerMask companionMask; // 同伴的LayerMask
    
    private Vector3 currentDirection; // 用于平滑方向过渡
    public float turnSmoothTime = 0.05f; // 转向平滑过渡的时间
    private float turnSmoothVelocity; // 用于SmoothDamp的速度
    
    // 保存最后的避障向量，以便在OnDrawGizmos中访问
    private Vector3 lastAvoidanceVector;
    private Vector3 lastSeparationVector;

    void Update()
    {
        Vector3 moveDirection = (player.position - transform.position).normalized; // 向玩家移动的向量
        moveDirection.z = 0;
        Vector3 avoidanceVector = CalculateAvoidanceVector(); // 避障向量
        avoidanceVector.z = 0;
        Vector3 separationVector = CalculateSeparationVector(); // 与同伴分离的向量
        separationVector.z = 0;
        
        // 将所有向量结合起来
        Vector3 finalDirection = moveDirection + avoidanceVector + separationVector;
        finalDirection.Normalize();
        finalDirection.z = 0;
        
        if (finalDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 移动敌人，确保不改变Z轴
        transform.position += transform.right * moveSpeed * Time.deltaTime;
    }

    Vector3 CalculateAvoidanceVector()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, avoidanceRadius, obstacleMask);
        Vector3 avoidanceVector = Vector3.zero;

        foreach (var hit in hits)
        {
            Vector3 directionToObstacle = transform.position - hit.transform.position;
            float distance = directionToObstacle.magnitude;

            // 基本的避障力量计算
            float strength = Mathf.Clamp(avoidanceStrength / Mathf.Max(distance, 1f), 0, avoidanceStrength);
            Vector3 basicAvoidance = directionToObstacle.normalized * strength;

            // 计算绕过障碍物的方向
            Vector3 playerDirection = (player.position - transform.position).normalized;
            Vector3 obstacleNormal = Vector3.Cross(Vector3.up, directionToObstacle).normalized;
            Vector3 lateralAvoidance = Vector3.Cross(obstacleNormal, playerDirection).normalized;

            // 检查应该向哪个方向绕过
            bool shouldAvoidToLeft = Vector3.Dot(lateralAvoidance, playerDirection) > 0;
            lateralAvoidance *= shouldAvoidToLeft ? 1 : -1;

            // 将基本避障向量与绕过障碍物的向量组合
            avoidanceVector += basicAvoidance + (lateralAvoidance * strength);
        }

        lastAvoidanceVector = avoidanceVector.normalized * avoidanceStrength;
        return lastAvoidanceVector;
    }

    Vector3 CalculateSeparationVector()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, separationRadius, companionMask);
        Vector3 separationVector = Vector3.zero;

        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject) // 确保不与自己计算分离向量
            {
                Vector3 directionToCompanion = transform.position - hit.transform.position;
                float distance = directionToCompanion.magnitude;
                // 当距离越小，分离向量越大
                separationVector += directionToCompanion.normalized / Mathf.Max(distance, 1f);
            }
        }
        lastSeparationVector = separationVector.normalized * separationStrength;
        return lastSeparationVector;
    }

    private void OnDrawGizmos()
    {
        // 画出向玩家移动的向量
        Gizmos.color = Color.green;
        Vector3 moveDirection = (player.position - transform.position).normalized;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection*0.2f ); 

        // 画出避障向量
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + lastAvoidanceVector*0.1f);

        // 画出分离向量
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + lastSeparationVector*0.1f);
    }
}
