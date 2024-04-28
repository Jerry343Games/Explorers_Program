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
        if (isSleeping)
        {
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            return;
        }
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
            MusicManager.Instance.PlaySound("怪物喷射");

            // 计算弹飞的方向
            Vector2 direction = (touchedCollision.transform.position - transform.position).normalized;

            // 给玩家一个弹飞的力
            touchedCollision.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
            touchedCollision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
    public void Move()
    {
        if (!canMove) return;
        Vector2 distance = (target.transform.position - transform.position);
        Vector2 direction = enemyAI.FinalMovement; // 获取朝向玩家的单位向量
        Vector2 targetDirection = distance.normalized;
        //如果距离大于射击范围，就进一步靠近玩家
        if (!isShooting&& Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) >= Mathf.Pow(shootDetectionRadius, 2))
        {
            rb.velocity = direction * moveSpeed; // 沿着朝向玩家的方向移动

            // 将人物的方向设置为计算得到的方向
            EnemyRotate();
            attackArea.SetActive(false);
            prepareTimer = 0;
        }
        else//如果进入射击范围，就执行射击逻辑
        {
            rb.velocity = Vector3.zero;
            //如果正在射
            if (isShooting)
            {
                
                
                rb.angularVelocity = Vector3.zero;
                if (shootTimer < shootTime)
                {
                    
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
            else if (!isShooting&&target!=null)//如果还没射
            {
                // 将人物的方向设置为计算得到的方向

                //gameObject.transform.right = direction;
                EnemyRotate();
                if (prepareTimer < prepareTime)//在准备射
                {
                    attackArea.SetActive(true);
                    float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(attackArea.transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                    attackArea.transform.rotation = Quaternion.Euler(0f, 0f, angle);


                    prepareTimer += Time.deltaTime;
                }
                else//开始射
                {
                    animator.Play("Attack");
                    attackArea.SetActive(false);
                    prepareTimer = 0;
                    isShooting = true;
                    //TODO
                    projectile = Instantiate(turbulencePrefab, transform.position, Quaternion.identity);
                    

                    // 获取预制体的 Transform 组件
                    Transform projectileTransform = projectile.transform;

                    // 将新物体的y方向设置为创建它的物体的x方向
                    projectileTransform.up = targetDirection;
                    // 让新物体朝着自己的y方向移动
                    projectile.GetComponent<Turbulence>().Shoot(shootForce);
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
