using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

//·ÉÊ¯

public class FlyingStones : Action
{

    private float stoneExitTime;

    private float stoneExitTimer;
    public override void OnStart()
    {
        GiantRockCrab.Instance.isPatrol = true;

        stoneExitTime = GiantRockCrab.Instance.stoneDuration;

        stoneExitTimer = stoneExitTime;

        GiantRockCrab.Instance.SpawnFlyingStones();
    }

    public override void OnEnd()
    {
    }

    public override TaskStatus OnUpdate()
    {
        if(stoneExitTimer>0)
        {
            stoneExitTimer -= Time.deltaTime;

            return TaskStatus.Running;
        }
        else
        {

            return TaskStatus.Success; ;
        }
    }

}
