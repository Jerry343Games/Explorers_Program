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

    Collider[] colliders; // 使用Collider数组替换Collider2D数组

    public override void Detect(AIData aiData)
    {
        colliders = Physics.OverlapSphere(transform.position, detectionRadius, layerMask); // 使用Physics.OverlapSphere替换Physics2D.OverlapCircleAll
        aiData.obstacles = colliders;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos == false)
            return;
        if (Application.isPlaying && colliders != null)
        {
            Gizmos.color = Color.red;
            foreach (Collider obstacleCollider in colliders) // 使用Collider替换Collider2D
            {
                Gizmos.DrawSphere(obstacleCollider.transform.position, 0.2f);
            }
        }
    }
}
