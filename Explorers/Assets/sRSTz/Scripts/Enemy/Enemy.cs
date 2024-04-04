using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 所有敌人都挂载这个脚本，不同种类的敌人的区别是里面的EnemySO不一样
/// </summary>
#region 影响因子相关
[System.Serializable]
public struct ImpactFactor
{
   /*public int angle0;
    public int angle30;
    public int angle60;
    public int angle90;*/
    public float[] factors;

    public ImpactFactor(float[] factors ) 
    {
        this.factors = factors;
        
    }
    /// <summary>
    /// 根据物体的标签和与敌人形成的向量，更新影响因子
    /// </summary>
    /// <param name="factorType"></param>
    /// <param name="vector"></param>
    public void UpdateFactor(string factorType,Vector2 vector)
    {
        //计算与物体最贴近的角度
        
        int closestAgle = FindAngle(vector);
        Debug.Log("clostAgle:" + closestAgle);
        //根据tag对印象因子进行修改
        switch (factorType)
        {
            case "Enemy":
                //Debug.Log("Vector:" + vector + " type:" + factorType);
                ChangeFactor(closestAgle, -0.4f, -0.3f, -0.2f, -0.1f);
                break;
            case "Player":
                //Debug.Log("Vector:" + vector + " type:" + factorType);
                ChangeFactor(closestAgle, 1, 0.8f, 0.6f, 0.4f);
                break;
            case "Wall":
                ChangeFactor(closestAgle, -1, -0.7f, -0.4f, -0.1f);
                break;
            case "Battery":
                //Debug.Log("Vector:" + vector + " type:" + factorType);
                ChangeFactor(closestAgle, 1, 0.8f, 0.6f, 0.4f);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 根据指向物体的方向，向外扩散三个方向 更改影响因子
    /// </summary>
    /// <param name="closestAgle"></param>
    /// <param name="main"></param>
    /// <param name="other1"></param>
    /// <param name="other2"></param>
    /// <param name="other3"></param>
    public void ChangeFactor(int closestAgle, float main, float other1, float other2, float other3)
    {
        float[] factorChange = new float[3] { other1, other2, other3 };
        factors[closestAgle] += main;
        for(int i = 0; i < 3; i++)
        {
            if (closestAgle - i+1 < 0)
            {
                factors[closestAgle - i+1 + 12] += factorChange[i];
                factors[closestAgle + i+1] += factorChange[i];
            }
            else
            {
                factors[closestAgle - i+1] += factorChange[i];
                if (closestAgle + i+1 > 12) factors[closestAgle + i+1 - 12] += factorChange[i];
                else factors[closestAgle +i+ 1] += factorChange[i];
            }
        }
        
        /*if (closestAgle - 2 < 0) factors[closestAgle - 2 + 12] -= other2;
        else factors[closestAgle - 2] += other2;
        if (closestAgle - 3 < 0) factors[closestAgle - 3 + 12] -= other3;
        else factors[closestAgle - 3]+= other3;*/
        

    }
    /// <summary>
    /// 根据向量计算出此向量与竖直向上方向的夹角（顺时针旋转的角度），并返回最接近的那个方向的index
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public int FindAngle(Vector2 vector)
    {
        float angle = Vector2.SignedAngle(Vector2.up, vector);
        angle = (angle + 360) % 360; // 转换为0到360度的范围
        Debug.Log("angle: " + angle);

        // 找到最接近的30度的倍数
        int closestMultiple = Mathf.RoundToInt(angle / 30); // 修正这一行

        return closestMultiple;
    }

    //todo 找到最大的因子的引用，同时清零数组
    public int FindMax()
    {
        float max = 0;
        int maxIndex = 0;
        for(int i = 0; i < factors.Length; i++)
        {
            if (factors[i] > max) { max = factors[i]; maxIndex = i; }
            factors[i] = 0;
        }
        //Debug.Log("maxindex: " + maxIndex);
        return maxIndex;
        
    }
}

#endregion
public class Enemy : MonoBehaviour
{
    // 可以之后加个存数据的结构体
    
    public float moveSpeed;
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

    [Header("影响因子相关")]
    public List<GameObject> detectedObjs = new();
    public Vector2 angleVector = Vector2.zero;
    public ImpactFactor impactFactor;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawnerPoint = gameObject.transform.position;
        impactFactor = new(new float[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
    }
    private void Update()
    {
        //TODO 每帧根据周边的物体计算影响因子，然后移动时根据影响因子移动
        impactFactor.UpdateFactor("Player",( target.transform.position)- transform.position );
        foreach(GameObject gameObject in detectedObjs)
        {
            Vector2 direction = gameObject.transform.position- transform.position ;
            impactFactor.UpdateFactor(gameObject.tag, direction);
        }
        Debug.Log(impactFactor.factors[0]+" "+ impactFactor.factors[1] + " " + impactFactor.factors[2] + " " + impactFactor.factors[3] + " " + impactFactor.factors[4] + " " + impactFactor.factors[5] + " " + impactFactor.factors[6] +
            " " + impactFactor.factors[7] + " " + impactFactor.factors[8] + " " + impactFactor.factors[9] + " " + impactFactor.factors[10] + " " + impactFactor.factors[11]);
        int maxFactor = impactFactor.FindMax();
        angleVector = AngleToVector(maxFactor * 30);
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
    Vector2 AngleToVector(float angle)
    {
        float radianAngle = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radianAngle);
        float y = Mathf.Sin(radianAngle);

        return new Vector2(x, y).normalized;
    }
    // 获取与自己距离最近的玩家
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

    //麻痹效果
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

            // 将人物的方向设置为计算得到的方向
            gameObject.transform.right = direction;
        }
    }
    public void EnemyRotate(Vector3 direction,float rotationSpeed)
    {
        float minAngleDifference = 0.2f;
        if (target != null)
        {
            
            direction.Normalize();

            // 计算目标旋转角度
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, direction);

            // 计算当前旋转角度与目标旋转角度之间的差值
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

            // 如果差值大于最小角度差，则进行旋转
            if (angleDifference > minAngleDifference)
            {
                // 使用 Slerp 实现平滑旋转
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
