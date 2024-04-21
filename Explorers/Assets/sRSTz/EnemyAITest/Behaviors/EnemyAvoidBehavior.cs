using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct EnemyTypeSpector
{
   public EnemyType enemyType;
   public float spector;
}
public class EnemyAvoidBehavior : SteeringBehaviour
{
    public List<EnemyTypeSpector> enemySpectors;
    [SerializeField]
    private float enemyRechedThreshold = 0.5f;
    [SerializeField]
    private float radius = 5f, agentColliderSize = 1f;
    //gizmo parameters
    float[] dangersResultTemp = null;
    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        //int[] avoid = new int[8];
        foreach(var enemyCollider in aiData.enemies)
        {
            Vector3 directionToEnemy = enemyCollider.ClosestPoint(transform.position) - transform.position;
            float distanceToEnemy = directionToEnemy.magnitude;
            
            float spcetor = enemySpectors.Find(enemySpector => enemySpector.enemyType == enemyCollider.GetComponent<Enemy>().enemyType).spector;
            float weight = distanceToEnemy <= agentColliderSize ? spcetor : (radius - distanceToEnemy) / radius;
            Vector3 directionToEnemyNormalized = directionToEnemy.normalized;
            for (int i = 0; i < Directions.eightDirections.Count; i++)
            {
                float result = Vector3.Dot(directionToEnemyNormalized, Directions.eightDirections[i]);

                float valueToPutIn = result * weight;

                //override value only if it is higher than the current one stored in the danger array
                //�Ȱ����е��˵Ļر����Ӽӵ�danger����ۼӣ������ߵ�1��
                danger[i] = danger[i] + valueToPutIn > 0.5f ? 0.5f: danger[i] + valueToPutIn;
               
                /*if (valueToPutIn > danger[i])
                {
                    danger[i] = valueToPutIn;
                }*/
            }
        }
        dangersResultTemp = danger;

        return (danger, interest);
    }
    
}
