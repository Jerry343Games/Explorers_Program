using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

//Ç¯»÷
public class PincerStrike : Action
{
    public override void OnStart()
    {
        Debug.Log("Ç¯»÷");

    }


    public override void OnEnd()
    {

    }

    public override TaskStatus OnUpdate()
    {

        if(GiantRockCrab.Instance.FindNearestPlayer())
        {
            GiantRockCrab.Instance.isPatrol = false;


            GiantRockCrab.Instance.PincerStrike();



            return TaskStatus.Success;
        }
        else
        {
            GiantRockCrab.Instance.isPatrol = true;
            return TaskStatus.Running;
        }


    }

    
}
