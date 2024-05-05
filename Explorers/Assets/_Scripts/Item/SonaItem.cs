using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SonaItem : MonoBehaviour
{

    private bool hasInit;
    private Transform _player;
    private Vector3 _nearestPos;
    public Image[] directionIndicators;// UI方向指示器数组，按照N, NE, E, SE, S, SW, W, NW排序
    
    
    void Update()
    {
        if (hasInit)
        {
            UpdateRadarDirection(_nearestPos);
            FollowPlayer();
        }
    }

    // 更新雷达指示方向
    public void UpdateRadarDirection(Vector3 nearestItemPos)
    {
        Vector3 directionToItem = (nearestItemPos - _player.position).normalized;
        float angle = Mathf.Atan2(directionToItem.x, directionToItem.y) * Mathf.Rad2Deg;

        Debug.Log(nearestItemPos);
        // 根据角度点亮相应的方向指示器
        HighlightDirection(angle);
    }
    
    void HighlightDirection(float angle)
    {
        // 确保角度在0-360度之间
        if (angle < 0) angle += 360;

        // 根据角度决定指示器亮起
        int index = Mathf.FloorToInt((angle + 22.5f) / 45) % 8;
        for (int i = 0; i < directionIndicators.Length; i++)
        {
            directionIndicators[i].enabled = (i == index);
        }
    }

    private void FollowPlayer()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_player.position);
        transform.position = screenPos;
    }
    
    /// <summary>
    /// 外部生成时初始化传入数据
    /// </summary>
    /// <param name="player"></param>
    /// <param name="nearestPos"></param>
    public void Init(Transform player,Vector3 nearestPos)
    {
        _player = player;
        _nearestPos = nearestPos;
        hasInit = true;
        Invoke("DestoryThis",10f);
    }

    private void DestoryThis()
    {
        Destroy(gameObject);
    }
}
