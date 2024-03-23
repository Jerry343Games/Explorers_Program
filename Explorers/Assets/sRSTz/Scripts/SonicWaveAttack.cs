using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicWaveAttack : MonoBehaviour
{
    
    private HashSet<Transform> detectedEnemies = new HashSet<Transform>();
    
    private float startRadius;
    private float targetRadius;
    private float enemyVertigoTime;
    private float currentRadius;
    private bool isStart = false;
    
    private float startTime;
    private float transitionDuration;
    private PlayerController user;
    private Vector3 startPosition;
    public void AttactStart(PlayerController user, float startRadius,float targetRadius,float vertigoTime,float transitionDuration)
    {
        this.startRadius = startRadius;
        this.targetRadius = targetRadius;
        enemyVertigoTime = vertigoTime;
        currentRadius = startRadius;
        this.user = user;
        startPosition = user.transform.position;
       // transform.parent = null;
        this.transitionDuration = transitionDuration;
        startTime = Time.time;
        isStart = true;
        Debug.Log("start");
        
    }

    void Update()
    {
        if (!isStart) return;
        transform.position = startPosition;
        
        Debug.Log(currentRadius);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentRadius);
        // 计算当前时间占过渡总时间的比例
        float t = Mathf.Clamp01((Time.time - startTime) / transitionDuration); // 计算过渡进度
        float easedT = Mathf.SmoothStep(0, 1, t); // 使用 SmoothStep 缓动函数

        currentRadius = Mathf.Lerp(startRadius, targetRadius, easedT);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy") && !detectedEnemies.Contains(col.transform))
            {
                Transform enemy = col.transform;
                detectedEnemies.Add(enemy);
                Debug.Log(enemy.name);
                // 调用敌人上的方法
                enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
                enemy.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                enemy.GetComponent<Rigidbody>().isKinematic = true;
                enemy.GetComponent<Enemy>().Vertigo(Vector3.zero, ForceMode.Force, enemyVertigoTime);

                // 可以在这里做其他处理
            }
        }

        if (t >= 1.0f)
        {
            Debug.Log("end");
            isStart = false;
            foreach(var enemy in detectedEnemies)
            {
                enemy.GetComponent<Rigidbody>().isKinematic = false;
            }
            detectedEnemies.Clear();
            transform.position = user.transform.position;
           // transform.parent = user.transform;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 设置颜色为半透明的蓝色
        Gizmos.color = new Color(0, 0, 1, 0.5f);

        // 绘制球体表示检测范围
        Gizmos.DrawSphere(transform.position, currentRadius);
    }
}
