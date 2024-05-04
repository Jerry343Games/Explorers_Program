using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStoryMove : MonoBehaviour
{
    public Transform targetPosition; // 目标位置
    public float speed = 5f; // 移动速度

    private Vector3 initialPosition;
    private bool isMoving = false;

    void Awake()
    {
        initialPosition = transform.position;
        MoveToTarget();
    }

    void Update()
    {
        if (isMoving)
        {
            // 计算当前位置到目标位置的方向
            Vector3 direction = (targetPosition.position - transform.position).normalized;

            // 计算当前帧的移动距离
            float distanceThisFrame = speed * Time.deltaTime;

            // 如果物体与目标点的距离小于这一帧的移动距离，直接到达目标点
            if (Vector3.Distance(transform.position, targetPosition.position) < distanceThisFrame)
            {
                transform.position = targetPosition.position;
                isMoving = false;

            }
            else
            {
                // 否则沿着方向移动
                transform.Translate(direction * distanceThisFrame, Space.World);
            }
        }
    }

    // 启动移动到目标位置的方法
    public void MoveToTarget()
    {
        isMoving = true;
    }

    // 重置位置到初始位置
    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}
