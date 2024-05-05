using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStoryMove : MonoBehaviour
{
    public Transform targetPosition; // Ŀ��λ��
    public float speed = 5f; // �ƶ��ٶ�

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
            // ���㵱ǰλ�õ�Ŀ��λ�õķ���
            Vector3 direction = (targetPosition.position - transform.position).normalized;

            // ���㵱ǰ֡���ƶ�����
            float distanceThisFrame = speed * Time.deltaTime;

            // ���������Ŀ���ľ���С����һ֡���ƶ����룬ֱ�ӵ���Ŀ���
            if (Vector3.Distance(transform.position, targetPosition.position) < distanceThisFrame)
            {
                transform.position = targetPosition.position;
                isMoving = false;

            }
            else
            {
                // �������ŷ����ƶ�
                transform.Translate(direction * distanceThisFrame, Space.World);
            }
        }
    }

    // �����ƶ���Ŀ��λ�õķ���
    public void MoveToTarget()
    {
        isMoving = true;
    }

    // ����λ�õ���ʼλ��
    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}
