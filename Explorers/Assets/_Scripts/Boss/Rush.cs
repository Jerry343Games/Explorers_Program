//using BehaviorDesigner.Runtime.Tasks;
//using UnityEngine;

////����
//public class Rush : Action
//{
//    private GameObject _target;

//    private float _rushTimer;

//    private float _rushTime;
//    public override void OnStart()
//    {
//    }

//    public override void OnEnd()
//    {
//        GiantRockCrab.Instance.SetMoveDirection(Vector3.zero);

//        GiantRockCrab.Instance.SetMoveSpeed(GiantRockCrab.Instance.normalSpeed);

//        GiantRockCrab.Instance.isPatrol = true;
//    }

//    public override TaskStatus OnUpdate()
//    {
//        _target = GiantRockCrab.Instance.FindNearestPlayer();

//        if(!_target)
//        {
//            return TaskStatus.Running;
//        }

//        GiantRockCrab.Instance.isPatrol = false;

//        _rushTime = GiantRockCrab.Instance.rushDuration;

//        _rushTimer = _rushTime;

//        Vector3 dir = (_target.transform.position - GiantRockCrab.Instance.transform.position).normalized;

//        GiantRockCrab.Instance.SetMoveDirection(dir);

//        GiantRockCrab.Instance.SetMoveSpeed(GiantRockCrab.Instance.rushSpeed);

//        //������










//        if (_rushTimer>0)
//        {
//            GiantRockCrab.Instance.Rush();
//            _rushTimer -= Time.deltaTime;
//            return TaskStatus.Running;
//        }
//        else
//        {
//            return TaskStatus.Success;
//        }
//    }

//}
