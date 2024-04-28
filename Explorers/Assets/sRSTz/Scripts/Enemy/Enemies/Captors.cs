using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captors : Enemy
{
    private GameObject currentCatchPlayer;
    private float damageCount=0;//累计受到的伤害
    private float maxHP;
    public int damageToBattery = 20;
    public int damageToPlayer = 10;
    public float killTime = 10;
    protected override void Awake()
    {
        base.Awake();
        aniEvent.OnEnemyAttackEvent += Attack;
        aniEvent.OnEnemyAttackEvent += AttackEnd;
        maxHP = HP;
    }
    private void FixedUpdate()
    {
        //GetClosestPlayer();
        //*************************************************
        if (isSleeping || !canMove)
        {
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            return;
        }
        if (currentCatchPlayer != null)
        {
            currentCatchPlayer.transform.position = transform.position;
        }
        Move();
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            animator.Play("Attack");
            Invoke(nameof(Attack), GetAnimatorLength(animator, "Attack") / 1.5f);
        }
    }*/
    public void Attack()
    {
        
        if (playersInAttackArea.Count == 0) return;

        if (currentCatchPlayer == null && canAttack)
        {
            MusicManager.Instance.PlaySound("怪物撕咬");
            CancelInvoke(nameof(KillPlayer));
            isAttack = true;
            currentCatchPlayer = playersInAttackArea[0];
            //Vertigo(-transform.forward * 5f, ForceMode.Impulse, 0.3f);
            PlayerController playerController = currentCatchPlayer.GetComponent<PlayerController>();
            playerController.Vertigo(Vector3.zero, ForceMode.Force, 100f);
            playerController.beCatched = true;
            Invoke(nameof(KillPlayer), killTime);

        }
        
        

    }
    public void KillPlayer()
    {

        Debug.Log("Kill");
        if (currentCatchPlayer != null)
        {
            PlayerController playerController = currentCatchPlayer.GetComponent<PlayerController>();
            if (currentCatchPlayer.CompareTag("Battery"))
            {
                playerController.TakeDamage(damageToBattery);
                
            }else if (currentCatchPlayer.CompareTag("Player"))
            {
                playerController.DisconnectRope();
                playerController.TakeDamage(damageToPlayer);
                
            }
            AttackStop();
        }
    }
    public void Move()
    {
        if (canMove)
        {
            if (!canAttack)//如果不能攻击
            {
                rb.velocity = enemyAI.FinalMovement * moveSpeed; // 沿着影响因子计算出的方向移动
                if (enemyAI.FinalMovement != Vector2.zero)
                {
                    float targetAngle = Mathf.Atan2(enemyAI.FinalMovement.y, enemyAI.FinalMovement.x) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, 0f, -angle);
                }
                // 判断 direction 是在 y 轴方向的左边还是右边
                if (isDefaultLeft)
                {
                    if (enemyAI.FinalMovement.x > 0 && !isFlipped) // 在 y 轴方向的右边，且当前没有翻转
                    {
                        // 翻转 Sprite
                        spriteRenderer.flipY = true;
                        isFlipped = true;
                    }
                    else if (enemyAI.FinalMovement.x < 0 && isFlipped) // 在 y 轴方向的左边，且当前已经翻转
                    {
                        // 取消翻转
                        spriteRenderer.flipY = false;
                        isFlipped = false;
                    }
                    return;
                }
                if (enemyAI.FinalMovement.x > 0 && isFlipped) // 在 y 轴方向的右边，且当前没有翻转
                {
                    // 翻转 Sprite
                    spriteRenderer.flipY = false;
                    isFlipped = false;
                }
                else if (enemyAI.FinalMovement.x < 0 && !isFlipped) // 在 y 轴方向的左边，且当前已经翻转
                {
                    // 取消翻转
                    spriteRenderer.flipY = true;
                    isFlipped = true;
                }
                return;
            }
            //Vector2 direction = (target.transform.position - transform.position).normalized; // 获取朝向玩家的单位向量

            rb.velocity = enemyAI.FinalMovement * moveSpeed; // 沿着影响因子计算出的方向移动
            
            
            EnemyRotate();
        }
       
    }
    public void AttackEnd()//攻击动画结束如果没咬到玩家，就重置能攻击bool 给动画用的
    {
        isAttack = false;
        
        if (currentCatchPlayer == null)
        {
            canAttack = true;
        }
    }
    public void AttackStop()//松口
    {
        if (currentCatchPlayer != null)
        {
            PlayerController playerController = currentCatchPlayer.GetComponent<PlayerController>();
            playerController.canMove = true;
            canAttack = true;
            isAttack = false;
            currentCatchPlayer = null;
            playerController.beCatched = false;
        }
    }
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        damageCount += damage;
        if (damageCount >= maxHP / 3)
        {
            AttackStop();
            damageCount = 0;
            Vertigo(Vector3.zero, ForceMode.Force, 3f);
        }
        

    }
    public override void Vertigo(Vector3 force, ForceMode forceMode = ForceMode.Impulse, float vertigoTime = 0.3F)
    {
        base.Vertigo(force, forceMode, vertigoTime);
        AttackStop();

    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (!isAttack && other.CompareTag("Player") || other.CompareTag("Battery"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null && (!player.canMainAttack && !player.canSecondaryAttack && !player.canMove)) return;
            playersInAttackArea.Add(other.gameObject);
            // animator.Play("Attack");
            //isAttack = true;
        }

    }


    protected override void OnTriggerExit(Collider other)
    {
        playersInAttackArea.Remove(other.gameObject);
    }

}
