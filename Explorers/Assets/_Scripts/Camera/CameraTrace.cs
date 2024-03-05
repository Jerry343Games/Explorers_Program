using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrace : MonoBehaviour
{
    public float smoothSpeed = 0.125f; // 相机移动的平滑速度
    public float maxZoom=20;
    public float minZoom;
    public float zoomLimiter = 50f;

    private float greatestDistance;
    
    void LateUpdate()
    {
        TraceCenter();
        CameraZoom();
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
        greatestDistance = GetGreatestDistance(allObjects);
        Vector3 centerPoint = GetCenterPoint(allObjects); // 计算所有对象的中心点

        Vector3 desiredPosition = centerPoint; // 相机的目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // 平滑移动到目标位置
        transform.position = smoothedPosition; // 更新相机位置
    }
    
    /// <summary>
    /// 控制相机拉远
    /// </summary>
    private void CameraZoom()
    {
        float zoom = Mathf.Lerp(minZoom, maxZoom, greatestDistance / zoomLimiter); // 根据玩家之间的最大距离调整相机的缩放
        float camX = Camera.main.transform.position.x;
        float camY = Camera.main.transform.position.y;
        float camZ = Camera.main.transform.position.z;
        Camera.main.transform.position = new Vector3(camX,camY,-zoom);
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
}
