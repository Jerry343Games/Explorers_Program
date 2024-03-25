using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraTrace : MonoBehaviour
{
    public static CameraTrace instance;
    
    public float smoothSpeed = 0.125f; // 相机移动的平滑速度
    public float maxZoom=20;
    public float minZoom;
    public float zoomLimiter = 50f;

    private float _greatestDistance;

    private float _defaultView;

    private Vector3 _centerPoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        _defaultView = Camera.main.fieldOfView;
    }

    void LateUpdate()
    {
        TraceCenter();
        CameraAutoZoom();
    }
    
    
    /// <summary>
    /// 追踪所有玩家合向量的中心
    /// </summary>
    private void TraceCenter()
    {
        // 查找所有标记为"Player"的游戏对象
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // 查找所有标记为"Battery"的游戏对象
        GameObject[] batteries = GameObject.FindGameObjectsWithTag("Battery");

        // 合并两个数组
        GameObject[] allObjects = new GameObject[players.Length + batteries.Length];
        
        players.CopyTo(allObjects, 0);
        batteries.CopyTo(allObjects, players.Length);

        if (allObjects.Length == 0) return; // 如果没有找到对象，不进行操作
        _greatestDistance = GetGreatestDistance(allObjects);
        _centerPoint = GetCenterPoint(allObjects); // 计算所有对象的中心点
        
        Vector3 desiredPosition = _centerPoint; // 相机的目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // 平滑移动到目标位置
        transform.position = smoothedPosition; // 更新相机位置
    }
    
    /// <summary>
    /// 控制相机拉远
    /// </summary>
    private void CameraAutoZoom()
    {
        float zoom = Mathf.Lerp(minZoom, maxZoom, _greatestDistance / zoomLimiter); // 根据玩家之间的最大距离调整相机的缩放
        float camX = Camera.main.transform.position.x;
        float camY = Camera.main.transform.position.y;
        float camZ = Camera.main.transform.position.z;
        Camera.main.transform.position = new Vector3(camX,camY,-zoom);
    }

    /// <summary>
    /// 相机震动
    /// </summary>
    /// <param name="duration">时长</param>
    /// <param name="strengh">力度</param>
    public void CameraShake(float duration,float strengh)
    {
        Camera.main.transform.DOShakePosition(duration, strengh);
    }

    /// <summary>
    /// 相机变焦
    /// </summary>
    /// <param name="duration">缩放到目标值时长</param>
    /// <param name="size">目标缩放值</param>
    /// <param name="pauseDuration">缩放到目标值时停顿时长</param>
    /// <param name="backDuration">缩放回正时长</param>
    public void CameraZoom(float duration,float size,float pauseDuration,float backDuration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DoCameraView(size,duration));
        sequence.AppendInterval(pauseDuration).Append(DoCameraView(_defaultView,backDuration));
        
    }
    
    /// <summary>
    /// 计算所有对象中心点
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    private Vector3 GetCenterPoint(GameObject[] objects)
    {
        if (objects.Length == 1)
        {
            return objects[0].transform.position; // 如果只有一个对象，中心点即为对象的位置
        }

        var bounds = new Bounds(objects[0].transform.position, Vector3.zero);
        for (int i = 1; i < objects.Length; i++)
        {
            bounds.Encapsulate(objects[i].transform.position); // 扩展边界以包含每个对象的位置
        }
        return bounds.center; // 返回边界的中心点，即所有对象的中心点
    }
    
    /// <summary>
    /// 获取玩家之间最大距离
    /// </summary>
    /// <param name="players"></param>
    /// <returns></returns>
    float GetGreatestDistance(GameObject[] players)
    {
        var bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 1; i < players.Length; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }
        return bounds.size.x; // 返回X轴上的距离作为最大距离，也可以选择使用边界的最大维度
    }

    private Tweener DoCameraView(float value, float time)
    {
        return DOTween.To(value =>
            {
                Camera.main.fieldOfView = value;
            },
            Camera.main.fieldOfView,
            value,
            time
        );
    }
}
