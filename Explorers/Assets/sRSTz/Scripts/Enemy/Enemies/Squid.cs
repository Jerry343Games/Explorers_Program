using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squid : Enemy
{
    // Start is called before the first frame update
    private PlayerController shieldDisintegrationPlayer = null;
    public float shieldDisintegrationTime = 5f;
    public float defenceDownRote = 1f;
    private void FixedUpdate()
    {
        //GetClosestPlayer();
        Move();
    }
    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawnerPoint = gameObject.transform.position;
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        isFlipped = spriteRenderer.flipY;
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Battery"))
        {

            touchedCollision = collision;
            Attack();
        }
    }
    public  void Attack()
    {

        if (touchedCollision != null && canAttack)
        {
            animator.Play("Attack");
            // 计算弹飞的方向
            Vector2 direction = (touchedCollision.transform.position - transform.position).normalized;
            shieldDisintegrationPlayer = touchedCollision.gameObject.GetComponent<PlayerController>();
            // 给玩家一个弹飞的力
            shieldDisintegrationPlayer.Vertigo(direction * force);
            shieldDisintegrationPlayer.TakeDamage(damage);

            Vertigo(-transform.forward * 5f, ForceMode.Impulse, 0.3f);
            shieldDisintegrationPlayer.DefenceDown(shieldDisintegrationTime, defenceDownRote);


        }
    }

    public void Move()
    {
        if (canMove)// 确保玩家存在
        {

            //Vector2 direction = (target.transform.position - transform.position).normalized; // 获取朝向玩家的单位向量

            rb.velocity = enemyAI.FinalMovement * moveSpeed; // 沿着影响因子计算出的方向移动

            // 将人物的方向设置为计算得到的方向
            //gameObject.transform.right = direction;
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
            if (enemyAI.FinalMovement.x < 0 && !isFlipped) // 在 y 轴方向的左边，且当前没有翻转
            {
                // 翻转 Sprite
                spriteRenderer.flipY = true;
                isFlipped = true;
            }
            else if (enemyAI.FinalMovement.x > 0 && isFlipped) // 在 y 轴方向的右边，且当前已经翻转
            {
                // 取消翻转
                spriteRenderer.flipY = false;
                isFlipped = false;
            }
        }
        /*
        else if(canMove) //如果丢失玩家并且能移动，那么回到出生点
        {
            ReturnSpawnpoint();
        }*/
    }
}
