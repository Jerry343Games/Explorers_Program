using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DashFishData", menuName = "EnemyData/DashFishData", order = 2)]
public class DashFish : EnemySO
{
    public float detectionRadius = 5f; // ���뾶
    private bool isDashing=false;
    public float prepareTime = 2f;
    private float prepareTimer = 0;
    public float dashTime = 3f;
    private float dashTimer = 0;
    public float dashForce = 3f;
    public override void Attack(Enemy enemy)
    {
        if (enemy.touchedCollision != null)
        {
            // ���㵯�ɵķ���
            Vector2 direction = (enemy.touchedCollision.transform.position - enemy.transform.position).normalized;

            // �����һ�����ɵ���
            enemy.touchedCollision.gameObject.GetComponent<Rigidbody>().AddForce(direction * enemy.force, ForceMode.Impulse);
        }
    }

    public override void FixUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void Move(Enemy enemy)
    {
        if (enemy.target == null) return; // ȷ����Ҵ���
        Vector2 distance = (enemy.target.transform.position - enemy.transform.position);
            Vector2 direction =distance. normalized; // ��ȡ������ҵĵ�λ����
        if (prepareTimer==0&& Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) >= Mathf.Pow(detectionRadius, 2)){
            enemy.rb.velocity = direction * enemy.moveSpeed; // ���ų�����ҵķ����ƶ�

            // ������ķ�������Ϊ����õ��ķ���
            enemy.gameObject.transform.right = direction;
        }
        else
        {
            // ������ķ�������Ϊ����õ��ķ���
            enemy.gameObject.transform.right = direction;
            //������ڳ��
            if (isDashing)
            {
                if (dashTimer < dashTime)
                {
                    dashTimer += Time.deltaTime;
                }
                else
                {
                    dashTimer = 0;
                    isDashing = false;
                    enemy.rb.velocity = new Vector3(0, 0, 0);
                }
            }
            else if(!isDashing)//�����û���
            {
                if (prepareTimer < prepareTime)
                {
                    prepareTimer += Time.deltaTime;
                }
                else
                {
                    prepareTimer = 0;
                    isDashing = true;
                    enemy.rb.AddForce(direction * dashForce, ForceMode.Impulse);
                }


                

            }
            





        }
            
        


    }

    public override void TakeDamage()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
    


}
