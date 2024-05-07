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
public enum EnemyType
{
    Piranha,
    AttackBatteryFish,
    ArcherFish,
    ElectricEel,
    Squid,
    DashFish
}
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
    
    public float stayTime = 4f;//如果丢失目标，原地等待的时间
    [HideInInspector]
    public bool isReturning = false;
    [HideInInspector]
    public float stayTimer = 0;
    
    public GameObject target=null;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public Collision touchedCollision;

    [HideInInspector]
    public Vector3 spawnerPoint;
    [HideInInspector]
    public bool canAttack = true;
    public EnemyAI enemyAI;//只负责寻路

    public float turnSmoothTime = 0.05f; // 转向平滑过渡的时间
    public float turnSmoothVelocity; // 用于SmoothDamp的速度

    public Animator animator;
    // 获取 SpriteRenderer 的当前翻转状态
    public bool isFlipped;
    public SpriteRenderer spriteRenderer;
    public bool isDefaultLeft = false;
    public EnemyType enemyType;

    [Header("沉睡相关")]
    public bool isSleeping;//勾选上之后，出生时会睡觉
    public float sleepDetectRadius=5f;//睡眠时的检测半径
   public float detectThread = 1f;
    public float awakeRadius = 6f;//叫醒周围敌人的半径
    [Header("修改后的攻击相关")]
    public float dashToAttackDetectRadius;//在此范围内速度加快
    public List<GameObject> playersInAttackArea;
    public AniEventControl aniEvent;
    public bool isAttack = false;
    public float fasterSpeed;
    //public float speedOffset = 3f;
    protected float defaultSpeed;
    public float slowSpeed;
    protected virtual void Awake()
    {

        //if(aniEvent!=null)


        defaultSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>();
        spawnerPoint = gameObject.transform.position;
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        isFlipped = spriteRenderer.flipY;
        if (isSleeping) InvokeRepeating(nameof(SleepAwakeCheck), 0, detectThread);
        originalColor = spriteRenderer.color;
    }
    private void Update()
    {
        //在这里检测距离，并改变速度  如果还没攻击，那么进入加速范围就加速，没进入就是正常速度
        if (!isAttack && GetClosestPlayer())
        {
            if(Vector2.Distance(GetClosestPlayer().transform.position, transform.position) < dashToAttackDetectRadius)
            {
                moveSpeed = fasterSpeed;
                ChangeDetectRadius(10f);
            }
        }
        else if(!isAttack)
        {
            moveSpeed = defaultSpeed;
        }


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
        if (playersInAttackArea.Count != 0&&!isAttack&&canAttack)
        {
            animator.Play("Attack");
            moveSpeed = slowSpeed;
            isAttack = true;
            rb.velocity = Vector3.zero;
            //**
            //canMove = false;
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
        if(closestPlayer!=null)
        target = closestPlayer;

        return closestPlayer;
    }

    public virtual void TakeDamage(int damage)
    { 
        HP -= damage;
        if (HP <= 0)
        {
            Dead();
            return;
        }
        FlashRed();
        if (isSleeping) { 
            StartledFromSleep();
            
        }
    }
    //敌人受击闪红相关
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private Color originalColor;
    public void FlashRed()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    public virtual void Dead()
    {
        Instantiate(Resources.Load<GameObject>("Effect/BloodExplosion"), transform.position, Quaternion.identity);
        MusicManager.Instance.PlaySound("怪物死亡");
        gameObject.SetActive(false);
    }
    public virtual void Vertigo(Vector3 force, ForceMode forceMode = ForceMode.Impulse, float vertigoTime = 0.3f)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
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
    
    public void EnemyRotate()
    {
        if (enemyAI.FinalMovement  != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(enemyAI.FinalMovement.y, enemyAI.FinalMovement.x) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        // 判断 direction 是在 y 轴方向的左边还是右边
        if (isDefaultLeft)
        {
            if (enemyAI.FinalMovement.x > 0 && !isFlipped) // 在 y 轴方向的右边，且当前没有翻转
            {
                // 翻转 Sprite
                spriteRenderer.flipY = true;
                isFlipped = true;
            }
            else if (enemyAI.FinalMovement.x < 0 && isFlipped) // 在 y 轴方向的左边，且当前已经翻转
            {
                // 取消翻转
                spriteRenderer.flipY = false;
                isFlipped = false;
            }
            return;
        }
        if (enemyAI.FinalMovement.x > 0 && isFlipped) // 在 y 轴方向的右边，且当前没有翻转
        {
            // 翻转 Sprite
            spriteRenderer.flipY = false;
            isFlipped = false;
        }
        else if (enemyAI.FinalMovement.x < 0 && !isFlipped) // 在 y 轴方向的左边，且当前已经翻转
        {
            // 取消翻转
            spriteRenderer.flipY = true;
            isFlipped = true;
        }
    }
    public static float GetAnimatorLength(Animator animator, string name)
    {
        //动画片段时间长度
        float length = 0;

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        
        foreach (AnimationClip clip in clips)
        {
            
            if (clip.name.Equals(name))
            {
                length = clip.length;
                break;
            }
        }
        return length;
    }
    /// <summary>
    /// 用某个轴去朝向物体
    /// </summary>
    /// <param name="tr_self">朝向的本体</param>
    /// <param name="lookPos">朝向的目标</param>
    /// <param name="directionAxis">方向轴，取决于你用那个方向去朝向</param>

    public  void AxisLookAt(Transform tr_self, Vector3 lookPos, Vector3 directionAxis)
    {
        var rotation = tr_self.rotation;
        var targetDir = lookPos - tr_self.position;
        //指定哪根轴朝向目标,自行修改Vector3的方向
        var fromDir = tr_self.rotation * directionAxis;
        //计算垂直于当前方向和目标方向的轴
        var axis = Vector3.Cross(fromDir, targetDir).normalized;
        //计算当前方向和目标方向的夹角
        var angle = Vector3.Angle(fromDir, targetDir);
        //将当前朝向向目标方向旋转一定角度，这个角度值可以做插值
        tr_self.rotation = Quaternion.AngleAxis(angle, axis) * rotation;
        tr_self.localEulerAngles = new Vector3(0, tr_self.localEulerAngles.y, 90);//后来调试增加的，因为我想让x，z轴向不会有任何变化
    }
    //检测一次范围内是否有玩家或者电池
    public void SleepAwakeCheck()
    {
        if ((GetClosestPlayer().transform.position - transform.position).magnitude < sleepDetectRadius)
        {
            
            StartledFromSleep();
        }
    }
    //强制唤醒  如果在某个行为发生了，并且敌人正在睡觉，则调用此方法强制唤醒
    public virtual void StartledFromSleep()
    {

        isSleeping = false;
        //TODO 短暂显示被惊动的图标
        Debug.Log("被惊动");
        CancelInvoke(nameof(SleepAwakeCheck));
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY|RigidbodyConstraints.FreezePositionZ|RigidbodyConstraints.FreezeRotationZ;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, awakeRadius);
        
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                Enemy enemy = hitCollider.GetComponent<Enemy>();
                if (enemy.isSleeping)

                    enemy.StartledFromSleep();


            }
        }


    }
    /// <summary>
    /// 改变检测范围
    /// </summary>
    /// <param name="radius"></param>
    public void ChangeDetectRadius(float radius)
    {
        TargetDetector targetDetector = GetComponentInChildren<TargetDetector>();
        targetDetector.ChangeRadius(radius);
    }
    //新攻击相关

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isAttack&& other.CompareTag("Player") || other.CompareTag("Battery"))
        {
            playersInAttackArea.Add(other.gameObject);
           // animator.Play("Attack");
            //isAttack = true;
        }
            
    }
    

    protected virtual void OnTriggerExit(Collider other)
    {
        playersInAttackArea.Remove(other.gameObject);
    }
    public void EnemyRotateWithFlip()
    {
        // 判断 direction 是在 y 轴方向的左边还是右边
        if (isDefaultLeft)
        {
            if (enemyAI.FinalMovement.x > 0 && !isFlipped) // 在 y 轴方向的右边，且当前没有翻转
            {
                // 翻转 Sprite
                spriteRenderer.flipX = true;
                isFlipped = true;
            }
            else if (enemyAI.FinalMovement.x < 0 && isFlipped) // 在 y 轴方向的左边，且当前已经翻转
            {
                // 取消翻转
                spriteRenderer.flipX = false;
                isFlipped = false;
            }
            return;
        }
        if (enemyAI.FinalMovement.x > 0 && isFlipped) // 在 y 轴方向的右边，且当前没有翻转
        {
            // 翻转 Sprite
            spriteRenderer.flipX = false;
            isFlipped = false;
        }
        else if (enemyAI.FinalMovement.x < 0 && !isFlipped) // 在 y 轴方向的左边，且当前已经翻转
        {
            // 取消翻转
            spriteRenderer.flipX = true;
            isFlipped = true;
        }
    }
}
