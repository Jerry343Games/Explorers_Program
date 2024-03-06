using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 所有敌人都挂载这个脚本，不同种类的敌人的区别是里面的EnemySO不一样
/// </summary>
public class Enemy : MonoBehaviour
{
    // 可以之后加个存数据的结构体
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

    
    // 获取与自己距离最近的玩家
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
