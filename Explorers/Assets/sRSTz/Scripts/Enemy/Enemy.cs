using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���е��˶���������ű�����ͬ����ĵ��˵������������EnemySO��һ��
/// </summary>
public class Enemy : MonoBehaviour
{
    // ����֮��Ӹ������ݵĽṹ��
    
    public float moveSpeed;
    public float force;
    public int HP = 10;
    public int damage = 10;
    public bool canMove = true;
    public float vertigoTime = 0.2f;
    private float vertigoTimer = 0;
    public float detectionRange = 5f;
    public float stayTime = 4f;//�����ʧĿ�꣬ԭ�صȴ���ʱ��
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

    public List<GameObject> detectedObjs = new();


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawnerPoint = gameObject.transform.position;
    }
    private void Update()
    {
        //TODO ÿ֡�����ܱߵ��������Ӱ�����ӣ�Ȼ���ƶ�ʱ����Ӱ�������ƶ�


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

    // ��ȡ���Լ�������������
    public GameObject GetClosestPlayer()
    {
        
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        if (PlayerManager.Instance.gamePlayers.Count == 0) return null;
        foreach (var character in PlayerManager.Instance.gamePlayers)
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

    //���Ч��
    public virtual void Paralysis(float continuedTime)
    {
        StartCoroutine(ParalysiseEffect(continuedTime));
    }

    IEnumerator ParalysiseEffect(float continuedTime)
    {
        moveSpeed *= 0.1f;
        yield return new WaitForSeconds(continuedTime);
        moveSpeed /= 0.1f;
    }
    public virtual void ReturnSpawnpoint()
    {
        Vector3 randomPosition = new Vector3(spawnerPoint.x + Random.Range(0,5f), spawnerPoint.y + Random.Range(0, 5f), spawnerPoint.z);
        if ( stayTimer < stayTime)
        {
            stayTimer += Time.fixedDeltaTime;
        }
        else
        {
            isReturning = true;

        }
        if ((randomPosition - transform.position).magnitude < 0.2f)
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

            // ������ķ�������Ϊ����õ��ķ���
            gameObject.transform.right = direction;
        }
    }
    public void EnemyRotate(Vector3 direction,float rotationSpeed)
    {
        float minAngleDifference = 0.2f;
        if (target != null)
        {
            
            direction.Normalize();

            // ����Ŀ����ת�Ƕ�
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, direction);

            // ���㵱ǰ��ת�Ƕ���Ŀ����ת�Ƕ�֮��Ĳ�ֵ
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // �����ֵ������С�ǶȲ�������ת
            if (angleDifference > minAngleDifference)
            {
                // ʹ�� Slerp ʵ��ƽ����ת
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        detectedObjs.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (detectedObjs.Contains(other.gameObject)) detectedObjs.Remove(other.gameObject);
    }


}
