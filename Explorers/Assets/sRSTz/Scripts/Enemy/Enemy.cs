using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���е��˶���������ű�����ͬ����ĵ��˵������������EnemySO��һ��
/// </summary>
public class Enemy : MonoBehaviour
{
    // ����֮��Ӹ������ݵĽṹ��
    public int moveSpeed;
    public EnemySO enemySO;
    public float force;
    [HideInInspector]
    public GameObject target=null;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Collision touchedCollision;
    private void FixedUpdate()
    {
        GetClosestPlayer();
        enemySO.Move(this);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            
            touchedCollision = collision;
            enemySO.Attack(this);
        }
    }
    // ��ȡ���Լ�������������
    public GameObject GetClosestPlayer()
    {
        
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        if (EnemyManager.Instance.players.Length == 0) return null;
        foreach (var character in EnemyManager.Instance.players)
        {
            //if (character.CompareTag("Enemy")) continue;
            float distanceToPlayer = Vector3.Distance(transform.position, character.transform.position);
            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestPlayer = character;
            }
            /*if (distanceToPlayer < chaState.property.attackRadius)
            {
                character.GetComponent<GamePlayerController>().TakeDamage(chaState.damageData);
            }*/
        }
        target = closestPlayer;

        return closestPlayer;
    }



}
