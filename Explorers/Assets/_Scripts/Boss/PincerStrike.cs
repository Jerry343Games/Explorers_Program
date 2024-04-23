using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Linq.Expressions;
using UnityEngine;

//ǯ��
public class PincerStrike : Action
{
    private GameObject _target;//ѡ������Ľ�ɫ
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

            //����Ŀ�����ڵ㣨��ȻĿ������Ѿ��뿪�ˣ�
            GiantRockCrab.Instance.PincerStrike();

            return TaskStatus.Success;

        }
        return TaskStatus.Running;


    }


}
