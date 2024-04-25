using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class JumpTest : MonoBehaviour
{
   
    public Transform startTrans;
    public Transform endTrans;
    public float height;
    private Vector3[] _path;
    public int resolution;
    public LineRenderer _lineRender;
    public bool move = false;
    public float time;
    public bool start = false;
    public AnimationCurve customEaseCurve = new AnimationCurve();
    //public Ease ease;
    // �����Զ����Ease���ߣ�ʵ���ȿ�Ȼ����Ȼ����Ч��
    private void Awake()
    {
        /*
        customEaseCurve.AddKey(0f, 0f); // ��ʼʱ�ٶ�Ϊ0
        customEaseCurve.AddKey(0.25f, 1.5f); // �ﵽ����ٶȵ�ʱ���
        customEaseCurve.AddKey(0.75f, 0.5f); // ���ٵ�ʱ���
        customEaseCurve.AddKey(1f, 1f); // ����ʱ�ٶ�Ϊ1*/
    }
    /// <param name="t">0��1��ֵ��0��ȡ���ߵ���㣬1������ߵ��յ�</param>
    /// <param name="start">���ߵ���ʼλ��</param>
    /// <param name="center">����������״�Ŀ��Ƶ�</param>
    /// <param name="end">���ߵ��յ�</param>
    public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }

    private void Update()
    {
        var startPoint = startTrans.position;
        var endPoint = endTrans.position;
        var bezierControlPoint = (startPoint + endPoint) * 0.5f + (Vector3.up * height);

        // �������嵱ǰ����߸߶ȵ���ֱ����
        float currentHeight = transform.position.y;
        float maxHeight = Mathf.Max(startPoint.y, bezierControlPoint.y, endPoint.y);
        float verticalDistance = maxHeight - currentHeight;

        // ������ֱ��������ƶ�ʱ�䣬ʹ�þ���Խ���ƶ�Խ��������Խ���ƶ�Խ��
        float adjustedTime = time * (1 - verticalDistance / maxHeight);

        if (start)
        {
            _path = new Vector3[resolution];
            for (int i = 0; i < resolution; i++)
            {
                var t = (i + 1) / (float)resolution;
                _path[i] = GetBezierPoint(t, startPoint, bezierControlPoint, endPoint);
            }
            _lineRender.positionCount = _path.Length;
            _lineRender.SetPositions(_path);
            start = false;
        }

        if (move)
        {
            transform.DOPath(_path, time).SetEase(customEaseCurve);
            
                move = false;
            
        }
    }




}
