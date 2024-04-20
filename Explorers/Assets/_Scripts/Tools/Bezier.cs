using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : Singleton<Bezier>
{

    private GameObject _obj;
    private float _speed;
    public void BezierCurve(Transform target,float r,GameObject obj,float speed)
    {
        _obj = obj;
        _speed = speed;
        StartCoroutine(BezierMove(obj.transform.position, GetRandomPoint(2),target));
    }
    

    public Vector3 GetRandomPoint(float r)
    {
        return transform.position+new Vector3(Random.Range(-r,r), Random.Range(-r, r),0);
    }
    IEnumerator BezierMove(Vector3 start,Vector3 mid,Transform target)
    {
        for(float i = 0; i <= 1; i += Time.deltaTime)
        {
            Vector3 p1 = Vector3.Lerp(start, mid, i);//起始点到中间点的插值
            Vector3 p2 = Vector3.Lerp(mid, target.position, i);//起始点到中间点的插值
            Vector3 p = Vector3.Lerp(p1, p2, i);//起始点到中间点的插值
            //移动物体 加yield return 可以保证MoveToPoint执行完一个之前不启动下一个
            yield return StartCoroutine(MoveToPoint(p));
        }
        yield return StartCoroutine(MoveToObject(target));

    }

    IEnumerator MoveToPoint(Vector3 p)
    {
        while(Vector3.Distance(_obj.transform.position,p)>0.1f)
        {
            Vector3 dir = p - _obj.transform.position;
            _obj.transform.position = Vector3.MoveTowards(_obj.transform.position, p, Time.deltaTime * _speed);
            yield return null;
        }
    }
    IEnumerator MoveToObject(Transform target)
    {
        while (Vector3.Distance(_obj.transform.position, target.position) > 0.1f)
        {
            Vector3 dir = target.position - _obj.transform.position;
            _obj.transform.position = Vector3.MoveTowards(_obj.transform.position, target.position, Time.deltaTime * _speed);
            yield return null;
        }
    }
}
