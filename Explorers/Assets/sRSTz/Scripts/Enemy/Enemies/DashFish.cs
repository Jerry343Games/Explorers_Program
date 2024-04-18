using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DashFish : Enemy
{
    public float dashDetectionRadius = 5f; // 冲刺的检测半径
    private bool isDashing=false;
    public float prepareTime = 2f;
    private float prepareTimer = 0;
    public float dashTime = 3f;
    private float dashTimer = 0;
    public float dashForce = 3f;
    private GameObject attackArea;
    private Vector3 attackAraeOffset;
    protected override void Awake()
    {
        base.Awake();
        attackArea = transform.GetChild(0).gameObject;
        attackAraeOffset = attackArea.transform.position -transform.position;
    }
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

   

    public  void Move()
    {
        if (!canMove) return; // 确保玩家存在
        Vector2 distance = (target.transform.position - transform.position);
        Vector2 direction = enemyAI.FinalMovement; // 获取朝向玩家的单位向量
        Vector2 targetDirection = distance.normalized;
        /*if (distance.magnitude > detectionRange && canMove)//如果丢失玩家并且能移动
        {
            ReturnSpawnpoint();
        }
        else*/
        if (prepareTimer==0&& Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) >= Mathf.Pow(dashDetectionRadius, 2)){
            rb.velocity = direction * moveSpeed; // 沿着朝向玩家的方向移动

            // 将人物的方向设置为计算得到的方向
            //gameObject.transform.right = direction;
            EnemyRotate();
        }
        else
        {
            
            //如果正在冲刺
            if (isDashing)
            {
                rb.angularVelocity = Vector3.zero;
                if (dashTimer < dashTime)
                {
                    dashTimer += Time.deltaTime;
                }
                else
                {
                    dashTimer = 0;
                    isDashing = false;
                    rb.velocity = new Vector3(0, 0, 0);
                }
            }
            else if(!isDashing)//如果还没冲刺
            {
                // 将人物的方向设置为计算得到的方向
                EnemyRotate();
                if (prepareTimer < prepareTime)
                {
                    attackArea.SetActive(true);
                    //AxisLookAt(attackArea.transform, (Vector2)attackArea.transform.position + direction, Vector3.right);
                    float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(attackArea. transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                    attackArea.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    attackArea.transform.position = transform.position + attackAraeOffset;
                    prepareTimer += Time.deltaTime;
                }
                else//准备完了就开始冲刺
                {
                    animator.Play("Attack");
                    attackArea.SetActive(false);
                    prepareTimer = 0;
                    isDashing = true;
                    rb.AddForce(targetDirection * dashForce, ForceMode.Impulse);
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
