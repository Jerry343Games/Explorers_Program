using BehaviorDesigner.Runtime.Tasks;
//����
public class SpitAcid : Action
{
    public override void OnStart()
    {
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
