using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Linq.Expressions;
using UnityEngine;

//钳击
public class PincerStrike : Action
{
    private GameObject _target;//选择最近的角色
    public override void OnStart()
    {
        _target = GiantRockCrab.Instance.FindNearestPlayer();

        GiantRockCrab.Instance.SetMoveSpeed(GiantRockCrab.Instance.closePlayerSpeed);

    }


    public override void OnEnd()
    {
        GiantRockCrab.Instance.SetMoveDirection(Vector3.zero);

        GiantRockCrab.Instance.SetMoveSpeed(GiantRockCrab.Instance.normalSpeed);
    }

    public override TaskStatus OnUpdate()
    {
        Vector3 targetPos = _target.transform.position;

        Vector3 dir = (targetPos - GiantRockCrab.Instance.transform.position).normalized;

        GiantRockCrab.Instance.SetMoveDirection(dir);

        if(Vector3.Distance(targetPos, GiantRockCrab.Instance.transform.position)<0.1f)
        {
            GiantRockCrab.Instance.SetMoveSpeed(0);

            //到达目标所在点（虽然目标可能已经离开了）
            GiantRockCrab.Instance.PincerStrike();

            return TaskStatus.Success;

        }
        return TaskStatus.Running;


    }


}
