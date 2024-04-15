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
            // ���㵯�ɵķ���
            Vector2 direction = (touchedCollision.transform.position - transform.position).normalized;
            shieldDisintegrationPlayer = touchedCollision.gameObject.GetComponent<PlayerController>();
            // �����һ�����ɵ���
            shieldDisintegrationPlayer.Vertigo(direction * force);
            shieldDisintegrationPlayer.TakeDamage(damage);

            Vertigo(-transform.forward * 5f, ForceMode.Impulse, 0.3f);
            shieldDisintegrationPlayer.DefenceDown(shieldDisintegrationTime, defenceDownRote);


        }
    }

    public void Move()
    {
        if (canMove)// ȷ����Ҵ���
        {

            //Vector2 direction = (target.transform.position - transform.position).normalized; // ��ȡ������ҵĵ�λ����

            rb.velocity = enemyAI.FinalMovement * moveSpeed; // ����Ӱ�����Ӽ�����ķ����ƶ�

            // ������ķ�������Ϊ����õ��ķ���
            //gameObject.transform.right = direction;
            if (isDefaultLeft)
            {
                if (enemyAI.FinalMovement.x > 0 && !isFlipped) // �� y �᷽����ұߣ��ҵ�ǰû�з�ת
                {
                    // ��ת Sprite
                    spriteRenderer.flipY = true;
                    isFlipped = true;
                }
                else if (enemyAI.FinalMovement.x < 0 && isFlipped) // �� y �᷽�����ߣ��ҵ�ǰ�Ѿ���ת
                {
                    // ȡ����ת
                    spriteRenderer.flipY = false;
                    isFlipped = false;
                }
                return;
            }
            if (enemyAI.FinalMovement.x < 0 && !isFlipped) // �� y �᷽�����ߣ��ҵ�ǰû�з�ת
            {
                // ��ת Sprite
                spriteRenderer.flipY = true;
                isFlipped = true;
            }
            else if (enemyAI.FinalMovement.x > 0 && isFlipped) // �� y �᷽����ұߣ��ҵ�ǰ�Ѿ���ת
            {
                // ȡ����ת
                spriteRenderer.flipY = false;
                isFlipped = false;
            }
        }
        /*
        else if(canMove) //�����ʧ��Ҳ������ƶ�����ô�ص�������
        {
            ReturnSpawnpoint();
        }*/
    }
}
