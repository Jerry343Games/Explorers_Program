using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbulenceSpawner : MonoBehaviour
{
   // public GameObject turbulencePrefab;
    private bool isShooting = false;
    public float prepareTime = 2f;
    private float prepareTimer = 0;
    public float shootTime = 3f;
    private float shootTimer = 0;
    public float shootForce = 3f;
    public GameObject projectile;//射出的东西
    
    public void StartShoot()
    {
        
        InvokeRepeating(nameof(Shoot), 1, prepareTime);
    }
    private void Shoot()
    {
        if (projectile.activeInHierarchy) projectile.SetActive(false);
        
        
        // 获取预制体的 Transform 组件
        Transform projectileTransform = projectile.transform;
        projectileTransform.position = transform.position;
        // 将新物体的y方向设置为创建它的物体的x方向
        projectileTransform.up = transform.right;
        projectile.SetActive(true);
        // 让新物体朝着自己的y方向移动
        projectile.GetComponent<Turbulence>().Shoot(shootForce);
    }
    

}
