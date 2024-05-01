using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
//Õ¬À·
public class SpitAcid : Action
{
    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
        
    }

    public override TaskStatus OnUpdate()
    {
        if(GiantRockCrab.Instance.FindNearestPlayer())
        {
            GiantRockCrab.Instance.isPatrol = false;
            GiantRockCrab.Instance.SpitAcid();
            return TaskStatus.Success;
        }
        else
        {
            GiantRockCrab.Instance.isPatrol = true;
            return TaskStatus.Running;

        }
    }
}
