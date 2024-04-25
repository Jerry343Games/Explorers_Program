using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Linq.Expressions;
using UnityEngine;

//钳击
public class PincerStrike : Action
{
    private Vector3 _target;//选择最近的角色
    public override void OnStart()
    {
        _target = GiantRockCrab.Instance.FindNearestPlayer().transform.position;

        GiantRockCrab.Instance.SetMoveSpeed(GiantRockCrab.Instance.closePlayerSpeed);

    }


    public override void OnEnd()
    {
        GiantRockCrab.Instance.SetMoveDirection(Vector3.zero);

        GiantRockCrab.Instance.SetMoveSpeed(GiantRockCrab.Instance.normalSpeed);
    }

    public override TaskStatus OnUpdate()
    {

        Vector3 dir = (_target - GiantRockCrab.Instance.transform.position).normalized;

        GiantRockCrab.Instance.SetMoveDirection(dir);

        if(Vector3.Distance(_target, GiantRockCrab.Instance.transform.position) < 5f)//大一点 因为怪物体型大
        {
            GiantRockCrab.Instance.SetMoveSpeed(0);

            //到达目标所在点（虽然目标可能已经离开了）
            GiantRockCrab.Instance.PincerStrike();

            return TaskStatus.Success;

        }
        return TaskStatus.Running;


    }


}
