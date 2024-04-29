using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkfish : Enemy
{
    [Header("MonkFish")]
    public bool canJump = true;
    public float jumpForce = 10f;
    public float waitTime = 2f;
    private bool isWaiting = false;
    protected override void Awake()
    {
        base.Awake();
        aniEvent.OnEnemyAttackEvent += Attack;
        aniEvent.EndEnemyAttackEvent += () => { isAttack = false; };
        isFlipped = spriteRenderer.flipX;
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
        Move();
        
    }
    
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player")||collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            Attack();
            
        }else if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Obstacle"))&&!isWaiting)
        {
            
            Invoke(nameof(CanJump), waitTime);
            rb.velocity = Vector3.zero;
            isWaiting = true;
        }
    }
    public void CanJump()
    {
        isWaiting = false;
        canJump = true;
    }
    public void Attack()
    {

        if (playersInAttackArea.Count == 0) return;
        foreach (var player in playersInAttackArea)
        {
            if (player != null && canAttack)
            {

                // 计算弹飞的方向
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // 给玩家一个弹飞的力
                player.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
                player.gameObject.GetComponent<PlayerController>().TakeDamage(damage);

                //Vertigo(-transform.forward * 5f, ForceMode.Impulse, 0.3f);

            }
        }
    }
    bool isJumpAttacking = false;
    public void Move()
    {
        if (!canMove) return;
        //rb.velocity = enemyAI.FinalMovement * moveSpeed; // 沿着影响因子计算出的方向移动
        Vector2 jumpDirection = enemyAI.FinalMovement.x < 0 ? Directions.eightDirections[7] : Directions.eightDirections[1];

        // 将人物的方向设置为计算得到的方向
        //gameObject.transform.right = direction;
        EnemyRotateWithFlip();
        if (canJump)
        {
            Debug.Log("Jump");
            animator.Play("Jump");
            rb.AddForce(jumpDirection * jumpForce,ForceMode.VelocityChange);
            canJump = false;

        }
        // 判断物体是往上运动还是往下运动
       /* if (rb.velocity.y > 0)
        {
            // 往上运动时逐渐减速
            rb.velocity = new Vector3(rb.velocity.x,rb. velocity.y - 0.3f, rb.velocity.z);
        }
        else
        {
            // 往下运动时加速
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + 0.3f, rb.velocity.z);
        }
       */
    }
    public void JumpAttack()
    {

        //执行跳跃


    }



    public override void StartledFromSleep()
    {
        isSleeping = false;
        //TODO 短暂显示被惊动的图标
        Debug.Log("被惊动");
        CancelInvoke(nameof(SleepAwakeCheck));
        rb = GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, awakeRadius);


        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                if (enemy.isSleeping)

                    enemy.StartledFromSleep();


            }
        }

    }
    //public bool isSpawning = false;
    public void SpawnEnemy()
    {

        EnemyManager.Instance.SpawnEnemyAfter();
        // animator.Play("Spawn");
    }

    public Collider[] colliders;
    public float flashRadius;
    
    public void Flash()
    {      
        canMove = false;
        colliders= Physics.OverlapSphere(transform.position, flashRadius);
        foreach(var obj in colliders)
        {
            if (obj.CompareTag("Player") || obj.CompareTag("Battery"))
            {
                // 故障几秒
            }
            else if (obj.CompareTag("Enemy"))
            {
                //敌人直接回血20
            }
        }
        //animator.Play("Flash");
    }
    public void FlashEnd()
    {
        canMove = true;
    }
    
}
