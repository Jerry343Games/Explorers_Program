using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archerfish : Enemy
{
    public float shootDetectionRadius = 8f; // 射击的检测半径
    private bool isShooting = false;
    public float prepareTime = 2f;
    private float prepareTimer = 0;
    public float shootTime = 3f;
    private float shootTimer = 0;
    public float shootForce = 3f;
    private GameObject attackArea;
    public GameObject turbulencePrefab;
    GameObject projectile;//射出的东西
    private void FixedUpdate()
    {
        GetClosestPlayer();
        Move();
    }
    protected override void Awake()
    {
        base.Awake();
        attackArea = transform.GetChild(0).gameObject;
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            Attack();
        }
    }
    public void Attack()
    {
        if (touchedCollision != null && canAttack)
        {
            // 计算弹飞的方向
            Vector2 direction = (touchedCollision.transform.position - transform.position).normalized;

            // 给玩家一个弹飞的力
            touchedCollision.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
            touchedCollision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
    public void Move()
    {
        if (target == null || !canMove) return; // 确保玩家存在
        Vector2 distance = (target.transform.position - transform.position);
        Vector2 direction = distance.normalized; // 获取朝向玩家的单位向量
        if (distance.magnitude > detectionRange && canMove)//如果丢失玩家并且能移动
        {
            ReturnSpawnpoint();
        }
        else
        if (prepareTimer == 0 && Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) >= Mathf.Pow(shootDetectionRadius, 2))
        {
            rb.velocity = direction * moveSpeed; // 沿着朝向玩家的方向移动

            // 将人物的方向设置为计算得到的方向
            gameObject.transform.right = direction;
        }
        else
        {
            rb.velocity = Vector3.zero;
            //如果正在冲刺
            if (isShooting)
            {
                
                
                rb.angularVelocity = Vector3.zero;
                if (shootTimer < shootTime)
                {
                    // 每帧让新物体朝着自己的y方向移动
                    projectile.transform.Translate(Vector3.up * shootForce * Time.deltaTime);
                    shootTimer += Time.deltaTime;
                }
                else
                {
                    shootTimer = 0;
                    Destroy(projectile);
                    projectile = null;
                    isShooting = false;
                    rb.velocity = new Vector3(0, 0, 0);
                    
                }
            }
            else if (!isShooting)//如果还没冲刺
            {
                // 将人物的方向设置为计算得到的方向
                gameObject.transform.right = direction;
                if (prepareTimer < prepareTime)
                {
                    attackArea.SetActive(true);
                    prepareTimer += Time.deltaTime;
                }
                else
                {
                    attackArea.SetActive(false);
                    prepareTimer = 0;
                    isShooting = true;
                    //TODO
                     projectile = Instantiate(turbulencePrefab, transform.position, Quaternion.identity);
                    

                    // 获取预制体的 Transform 组件
                    Transform projectileTransform = projectile.transform;

                    // 将新物体的y方向设置为创建它的物体的x方向
                    projectileTransform.up = transform.right;
                }

            }

        }

    }

    public override void Vertigo(Vector3 force, ForceMode forceMode = ForceMode.Impulse, float vertigoTime = 0.3F)
    {
        base.Vertigo(force, forceMode, vertigoTime);
        attackArea.SetActive(false);
    }

}
