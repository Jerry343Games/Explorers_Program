using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

//ǯ��
public class PincerStrike : Action
{
    private GameObject _target;//ѡ������Ľ�ɫ

    private bool findTarget;

    private bool actionSuccess;
    public override void OnStart()
    {
        Debug.Log("ǯ��");

        GiantRockCrab.Instance.isPatrol = false;
    }


    public override void OnEnd()
    {
        //GiantRockCrab.Instance.isPatrol = true;

        //GiantRockCrab.Instance.SetMoveDirection(Vector3.zero);

        //GiantRockCrab.Instance.SetMoveSpeed(GiantRockCrab.Instance.normalSpeed);
    }

    public override TaskStatus OnUpdate()
    {
        if(!findTarget)
        {
            _target = GiantRockCrab.Instance.FindNearestPlayer();

            if (!_target) return TaskStatus.Running;

            findTarget = true;

            return TaskStatus.Running;
        }
        else
        {

            GiantRockCrab.Instance.PincerStrike(_target);

            return TaskStatus.Success;

            ////ǯ����Ϊ
            //Vector3[] path = new Vector3[]
            //{
            //    GiantRockCrab.Instance.transform.position,
            //    _target.transform.position+new Vector3(0,2,0),
            //    GiantRockCrab.Instance.transform.position + new Vector3(_target.transform.position.x>GiantRockCrab.Instance.transform.position.x?2:-2,0,0)
            //};
            //Debug.Log(path);
            //Sequence s = DOTween.Sequence();
            //s.Append(GiantRockCrab.Instance.transform.DOPath(path, 2).OnComplete(() =>
            //{
            //    actionSuccess = true;
            //}));
            //if(actionSuccess)
            //{
            //    return TaskStatus.Success;
            //}
            //else
            //{
            //    return TaskStatus.Running; ;

            //}
        }

        //Vector3 dir = (_target - GiantRockCrab.Instance.transform.position).normalized;

        //GiantRockCrab.Instance.SetMoveDirection(dir);

        //if(Vector3.Distance(_target, GiantRockCrab.Instance.transform.position) < 5f)//��һ�� ��Ϊ�������ʹ�
        //{
        //    GiantRockCrab.Instance.SetMoveSpeed(0);

        //    //����Ŀ�����ڵ㣨��ȻĿ������Ѿ��뿪�ˣ�
        //    GiantRockCrab.Instance.PincerStrike();

        //    return TaskStatus.Success;

        //}
        //return TaskStatus.Running;


    }


}
