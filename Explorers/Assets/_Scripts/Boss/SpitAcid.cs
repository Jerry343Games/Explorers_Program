using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
//ÍÂËá
public class SpitAcid : Action
{
    public override void OnStart()
    {
        Debug.Log("ÍÂËá");
        GiantRockCrab.Instance.SpitAcid();
    }

    public override void OnEnd()
    {
        
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Running;
    }
}
