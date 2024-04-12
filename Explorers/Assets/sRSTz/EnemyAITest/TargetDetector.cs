using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField]
    private float targetDetectionRange = 5;

    [SerializeField]
    private LayerMask obstaclesLayerMask, playerLayerMask;
    [SerializeField]
    private LayerMask enemyLayerMask;//���������Χ���˵�
    [SerializeField]
    private float enemyDetectionRange = 5f;
    [SerializeField]
    private bool showGizmos = false;

    //gizmo parameters
    private List<Transform> colliders = new();

    public override void Detect(AIData aiData)
    {
        //Find out if player is near
        Collider[] playerColliderList =
            Physics.OverlapSphere(transform.position, targetDetectionRange, playerLayerMask);
        Collider[] enemyColliderList =
            Physics.OverlapSphere(transform.position, enemyDetectionRange, enemyLayerMask);
        if (playerColliderList != null)
        {
            colliders.Clear();
            foreach (Collider playerCollider in playerColliderList)
            {

                //Check if you see the player
                Vector3 direction = (playerCollider.transform.position - transform.position).normalized;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, targetDetectionRange, obstaclesLayerMask))
                {
                    //Make sure that the collider we see is on the "Player" layer
                    if ((playerLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
                    {
                        Debug.DrawRay(transform.position, direction * targetDetectionRange, Color.magenta);
                        colliders.Add(playerCollider.transform); /*= new List<Transform>() { playerCollider.transform };*/
                    }
                }
            }
        }
        else
        {
            //Enemy doesn't see the player
            colliders.Clear();
        }
        aiData.targets = colliders;
        if (enemyColliderList != null)
        {
            aiData.enemies.Clear();
            foreach(var enemyCollider in enemyColliderList)
            {
                aiData.enemies.Add(enemyCollider);
            }
        }
            
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false)
            return;

        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);

        if (colliders == null)
            return;
        Gizmos.color = Color.green;
        foreach (var item in colliders)
        {
            Gizmos.DrawSphere(item.position, 0.3f);
        }
    }
}