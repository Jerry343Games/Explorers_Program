using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : Detector
{
    [SerializeField]
    private float detectionRadius = 2;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private bool showGizmos = true;

    Collider[] colliders; // ʹ��Collider�����滻Collider2D����

    public override void Detect(AIData aiData)
    {
        colliders = Physics.OverlapSphere(transform.position, detectionRadius, layerMask); // ʹ��Physics.OverlapSphere�滻Physics2D.OverlapCircleAll
        aiData.obstacles = colliders;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;
        if (Application.isPlaying && colliders != null)
        {
            Gizmos.color = Color.red;
            foreach (Collider obstacleCollider in colliders) // ʹ��Collider�滻Collider2D
            {
                Gizmos.DrawSphere(obstacleCollider.transform.position, 0.2f);
            }
        }
    }
}
