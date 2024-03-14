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
    public int HP = 10;
    public int damage = 10;
    public bool canMove = true;
    public float vertigoTime = 0.2f;
    private float vertigoTimer = 0;
    public float detectionRange = 5f;
    public float stayTime = 4f;//如果丢失目标，原地等待的时间
    [HideInInspector]
    public bool isReturning = false;
    [HideInInspector]
    public float stayTimer = 0;
    [HideInInspector]
    public GameObject target=null;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Collision touchedCollision;

    [HideInInspector]
    public Vector3 spawnerPoint;
    [HideInInspector]
    public bool canAttack = true;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawnerPoint = gameObject.transform.position;
    }
    private void Update()
    {
        if (!canMove)
        {
            if (vertigoTimer >= vertigoTime)
            {
                canMove = true;
                vertigoTimer = 0;
            }
            else
            {
                vertigoTimer += Time.deltaTime;

            }
        }
    }

    // 获取与自己距离最近的玩家
    public GameObject GetClosestPlayer()
    {
        
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        if (EnemyManager.Instance.players.Count == 0) return null;
        foreach (var character in EnemyManager.Instance.players)
        {
            if (character.GetComponent<PlayerController>()==null|| character.GetComponent<PlayerController>().hasDead) continue;
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
        Instantiate(Resources.Load<GameObject>("Effect/BloodExplosion"), transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
    public virtual void Vertigo(Vector3 force, ForceMode forceMode = ForceMode.Impulse, float vertigoTime = 0.3f)
    {
        this.vertigoTime = vertigoTime;
        canMove = false;
        rb.AddForce(force, forceMode);
    }
    public virtual void ReturnSpawnpoint()
    {
        if ( stayTimer < stayTime)
        {
            stayTimer += Time.fixedDeltaTime;
        }
        else
        {
            isReturning = true;

        }
        if ((spawnerPoint - transform.position).magnitude < 0.2f)
        {
            isReturning = false;
            stayTimer = 0;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        if (isReturning)
        {
            Vector2 direction = (spawnerPoint - transform.position).normalized;

            rb.velocity = direction * moveSpeed;

            // 将人物的方向设置为计算得到的方向
            gameObject.transform.right = direction;
        }
    }
}
