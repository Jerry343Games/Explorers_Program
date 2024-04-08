using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archerfish : Enemy
{
    public float shootDetectionRadius = 8f; // ����ļ��뾶
    private bool isShooting = false;
    public float prepareTime = 2f;
    private float prepareTimer = 0;
    public float shootTime = 3f;
    private float shootTimer = 0;
    public float shootForce = 3f;
    private GameObject attackArea;
    public GameObject turbulencePrefab;
    GameObject projectile;//����Ķ���
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
            // ���㵯�ɵķ���
            Vector2 direction = (touchedCollision.transform.position - transform.position).normalized;

            // �����һ�����ɵ���
            touchedCollision.gameObject.GetComponent<PlayerController>().Vertigo(direction * force);
            touchedCollision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
    public void Move()
    {
        if (!canMove) return;
        Vector2 distance = (target.transform.position - transform.position);
        Vector2 direction = enemyAI.FinalMovement; // ��ȡ������ҵĵ�λ����
        //���������������Χ���ͽ�һ���������
        if (!isShooting&& Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) >= Mathf.Pow(shootDetectionRadius, 2))
        {
            rb.velocity = direction * moveSpeed; // ���ų�����ҵķ����ƶ�

            // ������ķ�������Ϊ����õ��ķ���
            EnemyRotate();
            attackArea.SetActive(false);
            prepareTimer = 0;
        }
        else//������������Χ����ִ������߼�
        {
            rb.velocity = Vector3.zero;
            //���������
            if (isShooting)
            {
                
                
                rb.angularVelocity = Vector3.zero;
                if (shootTimer < shootTime)
                {
                    // ÿ֡�������峯���Լ���y�����ƶ�
                    projectile.GetComponent<Turbulence>().Shoot(shootForce);
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
            else if (!isShooting&&target!=null)//�����û��
            {
                // ������ķ�������Ϊ����õ��ķ���

                //gameObject.transform.right = direction;
                EnemyRotate();
                if (prepareTimer < prepareTime)//��׼����
                {
                    attackArea.SetActive(true);
                    prepareTimer += Time.deltaTime;
                }
                else//��ʼ��
                {
                    
                    attackArea.SetActive(false);
                    prepareTimer = 0;
                    isShooting = true;
                    //TODO
                    projectile = Instantiate(turbulencePrefab, transform.position, Quaternion.identity);
                    

                    // ��ȡԤ����� Transform ���
                    Transform projectileTransform = projectile.transform;

                    // ���������y��������Ϊ�������������x����
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
