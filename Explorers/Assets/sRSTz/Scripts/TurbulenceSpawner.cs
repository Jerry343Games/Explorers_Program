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
    public GameObject projectile;//����Ķ���
    
    public void StartShoot()
    {
        
        InvokeRepeating(nameof(Shoot), 1, prepareTime);
    }
    private void Shoot()
    {
        if (projectile.activeInHierarchy) projectile.SetActive(false);
        
        
        // ��ȡԤ����� Transform ���
        Transform projectileTransform = projectile.transform;
        projectileTransform.position = transform.position;
        // ���������y��������Ϊ�������������x����
        projectileTransform.up = transform.right;
        projectile.SetActive(true);
        // �������峯���Լ���y�����ƶ�
        projectile.GetComponent<Turbulence>().Shoot(shootForce);
    }
    

}
