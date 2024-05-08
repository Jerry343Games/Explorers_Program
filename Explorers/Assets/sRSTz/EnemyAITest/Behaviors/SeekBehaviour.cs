using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class SeekBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float targetRechedThreshold = 0.5f;

    [SerializeField]
    private bool showGizmo = true;
   
    bool reachedLastTarget = true;

    //gizmo parameters
    private Vector2 targetPositionCached;
    private float[] interestsTemp;

    public bool isSeekBatteryFirst = false;
    public GameObject defaulttarget = null;
    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        //if we don't have a target stop seeking  如果找不到目标并且有默认目标，就先找默认目标
        //else set a new target
        if (aiData.currentTarget == null && defaulttarget != null)
        {
            aiData.currentTarget = defaulttarget.transform; targetPositionCached = aiData.currentTarget.position;
            Debug.Log(aiData.currentTarget + "##" + transform.position);
        }
        if (reachedLastTarget)
        {
            if (defaulttarget == null&&(aiData.targets == null || aiData.targets.Count <= 0))
            {
                
                aiData.currentTarget = null;

                return (danger, interest);
            }
            else
            {
                //优先找电池的代码先写到这了，水平不足
                reachedLastTarget = false;
                if (!isSeekBatteryFirst)
                {
                    aiData.currentTarget = aiData.targets.OrderBy
                    (target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
                }
                else
                {
                    Transform finalTarget=null;
                    foreach(var target in aiData.targets)
                    {
                        if (target.gameObject.CompareTag("Battery"))
                        {
                            finalTarget = target;
                            break;
                        }
                    }
                    if (finalTarget != null) aiData.currentTarget = finalTarget;

                    else aiData.currentTarget = aiData.targets.OrderBy
                    (target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
                }
                
            }

        }
        if (aiData.currentTarget == null && defaulttarget != null)
        {
            aiData.currentTarget = defaulttarget.transform; targetPositionCached = aiData.currentTarget.position;
            
        }
        //cache the last position only if we still see the target (if the targets collection is not empty)除非有默认目标
        if (aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
        {
            if (!isSeekBatteryFirst)
            {
                aiData.currentTarget = aiData.targets.OrderBy
                (target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
            }
            else
            {
                Transform finalTarget = null;
                foreach (var target in aiData.targets)
                {
                    if (target.gameObject.CompareTag("Battery"))
                    {
                        finalTarget = target;
                        break;
                    }
                }
                if (finalTarget != null) aiData.currentTarget = finalTarget;

                else aiData.currentTarget = aiData.targets.OrderBy
                (target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
            }
            
            targetPositionCached = aiData.currentTarget.position;
        }
        //if (aiData.currentTarget == null && defaulttarget != null) aiData.currentTarget = defaulttarget;
        if (aiData.currentTarget == null && defaulttarget != null)
        {
            aiData.currentTarget = defaulttarget.transform; targetPositionCached = aiData.currentTarget.position;
            Debug.Log(aiData.currentTarget + "##" + transform.position);
        }
        //First check if we have reached the target
        if (Vector2.Distance(transform.position, targetPositionCached) < targetRechedThreshold&&defaulttarget==null)
        {
            reachedLastTarget = true;
            aiData.currentTarget = null;
            return (danger, interest);
        }
        if (aiData.currentTarget == null && defaulttarget != null) { aiData.currentTarget = defaulttarget.transform; targetPositionCached = aiData.currentTarget.position;
            Debug.Log(aiData.currentTarget + "##" + transform.position); }
        //If we havent yet reached the target do the main logic of finding the interest directions
        Vector2 directionToTarget = (targetPositionCached - (Vector2)transform.position);
        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget.normalized, Directions.eightDirections[i]);

            //accept only directions at the less than 90 degrees to the target direction
            if (result > 0)
            {
                float valueToPutIn = result;
                if (valueToPutIn > interest[i])
                {
                    interest[i] = valueToPutIn;
                }

            }
        }
        interestsTemp = interest;
        return (danger, interest);
    }
    public void DodgePlayer()
    {

    }
    private void OnDrawGizmos()
    {

        if (showGizmo == false)
            return;
        Gizmos.DrawSphere(targetPositionCached, 0.2f);
        
        if (Application.isPlaying && interestsTemp != null)
        {
            if (interestsTemp != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < interestsTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * interestsTemp[i]*2);
                }
                if (reachedLastTarget == false)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(targetPositionCached, 0.1f);
                }
            }
        }
    }
}
