using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PiranhaData",menuName ="EnemyData/PiranhaData",order =1)]
public class Piranha : EnemySO
{

    public override void Attack(Enemy enemy)
    {
        
        if (enemy.touchedCollision != null)
        {
            
            // ���㵯�ɵķ���
            Vector2 direction = (enemy.touchedCollision. transform.position- enemy.transform.position).normalized;

            // �����һ�����ɵ���
            enemy.touchedCollision.gameObject.GetComponent<Rigidbody>().AddForce(direction * enemy.force, ForceMode.Impulse);
        }
    }

    public override void Move(Enemy enemy)
    {
        if (enemy.target != null) // ȷ����Ҵ���
        {
            Vector2 direction = (enemy.target.transform.position - enemy.transform.position).normalized; // ��ȡ������ҵĵ�λ����
            enemy.rb.velocity = direction * enemy.moveSpeed; // ���ų�����ҵķ����ƶ�

            // ������ķ�������Ϊ����õ��ķ���
            enemy.gameObject.transform.right = direction;
        }
    }

    public override void TakeDamage()
    {
        
    }

    public override void Update()
    {
        
    }
    

    public override void FixUpdate()
    {
        throw new System.NotImplementedException();
    }
}
