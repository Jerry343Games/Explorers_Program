using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
//����
public class SpitAcid : Action
{
    public override void OnStart()
    {
        Debug.Log("����");
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
