using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AISamples : MonoBehaviour
{
    public float detectionRadius;//检测半径
    public float moveSpeed;//速度
    public GameObject player;
    public float separationDistance;//AI想保持的最小距离
    public int index;//自己在队列中的序号
    private Rigidbody _rb;
    private float _speedFactor;//随机速度因子，视觉上增加多样性并可能避免同步移动造成的视觉单调性
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _speedFactor = Random.Range(0.8f, 1.2f);
        StartCoroutine(UpdateSpeedFactorCoroutine());
    }

    void Update()
    {
        // 检测周围的对象
        List<Transform> nearbyObjects = DetectNearbyObjects(transform, detectionRadius);
        
        // 计算基本行为
        Vector3 separationVector = CalculateSeparationVector(nearbyObjects,separationDistance);
        Vector3 alignmentVector = CalculateAlignmentVector(nearbyObjects);
        Vector3 cohesionVector = CalculateCohesionVector(nearbyObjects);
        Vector3 avoidObstacleVector = AvoidObstacles(transform, detectionRadius);

        // 应用最终移动方向和速度
        Vector3 formationPosition = GetFormationPosition(player.transform, index, 0.5f, 9);
        ApplyMovement(formationPosition, separationVector, moveSpeed,2f);
    }
    
    /// <summary>
    /// 通过Physics.OverlapSphere方法实现检测给定范围内的所有物体。
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="detectionRadius"></param>
    /// <returns></returns>
    List<Transform> DetectNearbyObjects(Transform agent, float detectionRadius)
    {
        List<Transform> detectedObjects = new List<Transform>();
        Collider[] hits = Physics.OverlapSphere(agent.position, detectionRadius);
        foreach (var hit in hits)
        {
            if (hit.transform != agent) // 排除自身
            {
                detectedObjects.Add(hit.transform);
            }
        }
        return detectedObjects;
    }
    
    /// <summary>
    /// 避障
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="detectionRadius"></param>
    /// <returns></returns>
    Vector3 AvoidObstacles(Transform agent, float detectionRadius)
    {
        RaycastHit hit;
        if (Physics.Raycast(agent.position, agent.forward, out hit, detectionRadius))
        {
            if (hit.collider.tag == "Barrier") // 假设所有障碍物都标记为"Obstacle"
            {
                Vector3 avoidDirection = Vector3.Reflect(agent.forward, hit.normal);
                return avoidDirection.normalized;
            }
        }
        return Vector3.zero;
    }
    
    /// <summary>
    /// 向玩家移动
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="player"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    Vector3 MoveTowardsPlayer(Transform enemy, Transform player, float speed)
    {
        Vector3 directionToPlayer = (player.position - enemy.position).normalized;
        // 此处可以添加更多逻辑，如检查玩家是否在一定范围内等
        return directionToPlayer * speed;
    }
    
    Vector3 CalculateSeparationVector(List<Transform> neighbors, float desiredSeparationDistance)
    {
        Vector3 separationVector = Vector3.zero;
        int count = 0;

        foreach (Transform neighbor in neighbors)
        {
            Vector3 diff = transform.position - neighbor.position;
            float distance = diff.magnitude;

            if (distance > 0 && distance < desiredSeparationDistance)
            {
                // 与邻居的距离越近，推力越大
                Vector3 pushForce = diff.normalized / distance;
                separationVector += pushForce;
                count++;
            }
        }

        if (count > 0)
        {
            separationVector /= count;
        }

        return separationVector;
    }


    Vector3 CalculateAlignmentVector(List<Transform> neighbors)
    {
        Vector3 alignVector = Vector3.zero;
        foreach (Transform neighbor in neighbors)
        {
            alignVector += neighbor.forward; // 取得所有邻居的前进方向
        }
        if (neighbors.Count > 0)
        {
            alignVector /= neighbors.Count; // 取平均值
            return alignVector.normalized;
        }
        return transform.forward; // 没有邻居时保持当前方向
    }

    Vector3 CalculateCohesionVector(List<Transform> neighbors)
    {
        Vector3 cohesionVector = Vector3.zero;
        if (neighbors.Count == 0) return cohesionVector;

        foreach (Transform neighbor in neighbors)
        {
            cohesionVector += neighbor.position; // 所有邻居的位置
        }
        cohesionVector /= neighbors.Count; // 计算平均位置
        cohesionVector = cohesionVector - transform.position; // 朝向平均位置的向量
        return cohesionVector.normalized;
    }
    
    /// <summary>
    /// 应用所有影响向量
    /// </summary>
    /// <param name="movementDirection"></param>
    void ApplyMovement(Vector3 targetPosition, Vector3 separationVector, float moveSpeed,float separationWeight=2)
    {
        // 计算到目标的距离
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 基于距离调整移动速度，使得AI离目标越远时移动越快
        float adjustedSpeed = 1;

        // 计算最终的移动方向
        Vector3 finalDirection = ((targetPosition - transform.position).normalized + separationVector).normalized;
        
        // 当AI足够接近目标位置时，减少移动速度以避免抖动
        if (distanceToTarget < 0.1f)
        {
            adjustedSpeed = 0;
        }

        // 应用计算出的速度和方向
        _rb.velocity = finalDirection * adjustedSpeed*_speedFactor;

        // 确保AI朝向移动方向
        if (finalDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(finalDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 0.1f);
        }
    }
    
    
    /// <summary>
    /// 列队
    /// </summary>
    /// <param name="player"></param>
    /// <param name="index"></param>
    /// <param name="radius"></param>
    /// <param name="totalAgents"></param>
    /// <returns></returns>
    Vector3 GetFormationPosition(Transform player, int index, float radius, int totalAgents)
    {
        float anglePerAgent = 360f / totalAgents;
        float angle = anglePerAgent * index;
        // 在XY平面上计算偏移
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
        return player.position + offset;
    }
    
    IEnumerator UpdateSpeedFactorCoroutine()
    {
        while (true)
        {
            // 每隔1秒随机更新speedFactor
            _speedFactor = Random.Range(0.7f, 1.3f);
        
            // 等待1秒
            yield return new WaitForSeconds(0.5f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,detectionRadius);
    }

}


