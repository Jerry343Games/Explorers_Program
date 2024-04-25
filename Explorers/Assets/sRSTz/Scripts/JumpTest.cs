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
    // 定义自定义的Ease曲线，实现先快然后慢然后快的效果
    private void Awake()
    {
        /*
        customEaseCurve.AddKey(0f, 0f); // 开始时速度为0
        customEaseCurve.AddKey(0.25f, 1.5f); // 达到最大速度的时间点
        customEaseCurve.AddKey(0.75f, 0.5f); // 减速的时间点
        customEaseCurve.AddKey(1f, 1f); // 结束时速度为1*/
    }
    /// <param name="t">0到1的值，0获取曲线的起点，1获得曲线的终点</param>
    /// <param name="start">曲线的起始位置</param>
    /// <param name="center">决定曲线形状的控制点</param>
    /// <param name="end">曲线的终点</param>
    public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }

    private void Update()
    {
        var startPoint = startTrans.position;
        var endPoint = endTrans.position;
        var bezierControlPoint = (startPoint + endPoint) * 0.5f + (Vector3.up * height);

        // 计算物体当前与最高高度的竖直距离
        float currentHeight = transform.position.y;
        float maxHeight = Mathf.Max(startPoint.y, bezierControlPoint.y, endPoint.y);
        float verticalDistance = maxHeight - currentHeight;

        // 根据竖直距离调整移动时间，使得距离越高移动越慢，距离越低移动越快
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
