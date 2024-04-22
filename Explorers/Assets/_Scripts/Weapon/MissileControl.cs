using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControl : MonoBehaviour
{
    public GameObject target; // 公共目标，所有导弹将追踪这个目标
    public float speed = 5.0f; // 所有导弹的统一速度
    public float rotateSpeed = 200.0f; // 所有导弹的统一旋转速度

    void Start()
    {
        // 获取所有子物体上的MissileTracker组件
        Missile[] missiles = GetComponentsInChildren<Missile>();

        // 为每个导弹的MissileTracker组件设置属性
        foreach (Missile missile in missiles)
        {
            missile._target = target;
            missile._speed = speed;
            missile._rotateSpeed = rotateSpeed;
        }
        
        Invoke("DestroyThis",5f);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
