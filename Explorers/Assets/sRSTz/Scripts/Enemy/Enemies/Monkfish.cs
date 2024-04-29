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

                // ���㵯�ɵķ���
                Vector2 direction = (player.transform.position - transform.position).normalized;

                // �����һ�����ɵ���
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
        //rb.velocity = enemyAI.FinalMovement * moveSpeed; // ����Ӱ�����Ӽ�����ķ����ƶ�
        Vector2 jumpDirection = enemyAI.FinalMovement.x < 0 ? Directions.eightDirections[7] : Directions.eightDirections[1];

        // ������ķ�������Ϊ����õ��ķ���
        //gameObject.transform.right = direction;
        EnemyRotateWithFlip();
        if (canJump)
        {
            Debug.Log("Jump");
            animator.Play("Jump");
            rb.AddForce(jumpDirection * jumpForce,ForceMode.VelocityChange);
            canJump = false;

        }
        // �ж������������˶����������˶�
       /* if (rb.velocity.y > 0)
        {
            // �����˶�ʱ�𽥼���
            rb.velocity = new Vector3(rb.velocity.x,rb. velocity.y - 0.3f, rb.velocity.z);
        }
        else
        {
            // �����˶�ʱ����
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + 0.3f, rb.velocity.z);
        }
       */
    }
    public void JumpAttack()
    {

        //ִ����Ծ


    }



    public override void StartledFromSleep()
    {
        isSleeping = false;
        //TODO ������ʾ��������ͼ��
        Debug.Log("������");
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
                // ���ϼ���
            }
            else if (obj.CompareTag("Enemy"))
            {
                //����ֱ�ӻ�Ѫ20
            }
        }
        //animator.Play("Flash");
    }
    public void FlashEnd()
    {
        canMove = true;
    }
    
}
