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
    public float force;
    [HideInInspector]
    public GameObject target=null;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Collision touchedCollision;
    public int HP = 10;
    public int damage = 10;

    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    // ��ȡ���Լ�������������
    public GameObject GetClosestPlayer()
    {
        
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        if (EnemyManager.Instance.players.Count == 0) return null;
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

    public virtual void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0) Dead();
    }
    public virtual void Dead()
    {
        gameObject.SetActive(false);
    }
}
