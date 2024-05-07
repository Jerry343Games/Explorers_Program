using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���е��˶���������ű�����ͬ����ĵ��˵������������EnemySO��һ��
/// </summary>
#region Ӱ���������
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
    /// ��������ı�ǩ��������γɵ�����������Ӱ������
    /// </summary>
    /// <param name="factorType"></param>
    /// <param name="vector"></param>
    public void UpdateFactor(string factorType,Vector2 vector)
    {
        //�����������������ĽǶ�
        
        int closestAgle = FindAngle(vector);
        Debug.Log("clostAgle:" + closestAgle);
        //����tag��ӡ�����ӽ����޸�
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
    /// ����ָ������ķ���������ɢ�������� ����Ӱ������
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
    /// �����������������������ֱ���Ϸ���ļнǣ�˳ʱ����ת�ĽǶȣ�����������ӽ����Ǹ������index
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public int FindAngle(Vector2 vector)
    {
        float angle = Vector2.SignedAngle(Vector2.up, vector);
        angle = (angle + 360) % 360; // ת��Ϊ0��360�ȵķ�Χ
        Debug.Log("angle: " + angle);

        // �ҵ���ӽ���30�ȵı���
        int closestMultiple = Mathf.RoundToInt(angle / 30); // ������һ��

        return closestMultiple;
    }

    //todo �ҵ��������ӵ����ã�ͬʱ��������
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
    // ����֮��Ӹ������ݵĽṹ��
    
    public float moveSpeed;
    public float force;
    public int HP = 10;
    public int damage = 10;
    public bool canMove = true;
    public float vertigoTime = 0.2f;
    private float vertigoTimer = 0;
    
    public float stayTime = 4f;//�����ʧĿ�꣬ԭ�صȴ���ʱ��
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
    public EnemyAI enemyAI;//ֻ����Ѱ·

    public float turnSmoothTime = 0.05f; // ת��ƽ�����ɵ�ʱ��
    public float turnSmoothVelocity; // ����SmoothDamp���ٶ�

    public Animator animator;
    // ��ȡ SpriteRenderer �ĵ�ǰ��ת״̬
    public bool isFlipped;
    public SpriteRenderer spriteRenderer;
    public bool isDefaultLeft = false;
    public EnemyType enemyType;

    [Header("��˯���")]
    public bool isSleeping;//��ѡ��֮�󣬳���ʱ��˯��
    public float sleepDetectRadius=5f;//˯��ʱ�ļ��뾶
   public float detectThread = 1f;
    public float awakeRadius = 6f;//������Χ���˵İ뾶
    [Header("�޸ĺ�Ĺ������")]
    public float dashToAttackDetectRadius;//�ڴ˷�Χ���ٶȼӿ�
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
        //����������룬���ı��ٶ�  �����û��������ô������ٷ�Χ�ͼ��٣�û������������ٶ�
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
    //�����ܻ��������
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
        MusicManager.Instance.PlaySound("��������");
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
    
    public void EnemyRotate()
    {
        if (enemyAI.FinalMovement  != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(enemyAI.FinalMovement.y, enemyAI.FinalMovement.x) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        // �ж� direction ���� y �᷽�����߻����ұ�
        if (isDefaultLeft)
        {
            if (enemyAI.FinalMovement.x > 0 && !isFlipped) // �� y �᷽����ұߣ��ҵ�ǰû�з�ת
            {
                // ��ת Sprite
                spriteRenderer.flipY = true;
                isFlipped = true;
            }
            else if (enemyAI.FinalMovement.x < 0 && isFlipped) // �� y �᷽�����ߣ��ҵ�ǰ�Ѿ���ת
            {
                // ȡ����ת
                spriteRenderer.flipY = false;
                isFlipped = false;
            }
            return;
        }
        if (enemyAI.FinalMovement.x > 0 && isFlipped) // �� y �᷽����ұߣ��ҵ�ǰû�з�ת
        {
            // ��ת Sprite
            spriteRenderer.flipY = false;
            isFlipped = false;
        }
        else if (enemyAI.FinalMovement.x < 0 && !isFlipped) // �� y �᷽�����ߣ��ҵ�ǰ�Ѿ���ת
        {
            // ȡ����ת
            spriteRenderer.flipY = true;
            isFlipped = true;
        }
    }
    public static float GetAnimatorLength(Animator animator, string name)
    {
        //����Ƭ��ʱ�䳤��
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
    /// ��ĳ����ȥ��������
    /// </summary>
    /// <param name="tr_self">����ı���</param>
    /// <param name="lookPos">�����Ŀ��</param>
    /// <param name="directionAxis">�����ᣬȡ���������Ǹ�����ȥ����</param>

    public  void AxisLookAt(Transform tr_self, Vector3 lookPos, Vector3 directionAxis)
    {
        var rotation = tr_self.rotation;
        var targetDir = lookPos - tr_self.position;
        //ָ���ĸ��ᳯ��Ŀ��,�����޸�Vector3�ķ���
        var fromDir = tr_self.rotation * directionAxis;
        //���㴹ֱ�ڵ�ǰ�����Ŀ�귽�����
        var axis = Vector3.Cross(fromDir, targetDir).normalized;
        //���㵱ǰ�����Ŀ�귽��ļн�
        var angle = Vector3.Angle(fromDir, targetDir);
        //����ǰ������Ŀ�귽����תһ���Ƕȣ�����Ƕ�ֵ��������ֵ
        tr_self.rotation = Quaternion.AngleAxis(angle, axis) * rotation;
        tr_self.localEulerAngles = new Vector3(0, tr_self.localEulerAngles.y, 90);//�����������ӵģ���Ϊ������x��z���򲻻����κα仯
    }
    //���һ�η�Χ���Ƿ�����һ��ߵ��
    public void SleepAwakeCheck()
    {
        if ((GetClosestPlayer().transform.position - transform.position).magnitude < sleepDetectRadius)
        {
            
            StartledFromSleep();
        }
    }
    //ǿ�ƻ���  �����ĳ����Ϊ�����ˣ����ҵ�������˯��������ô˷���ǿ�ƻ���
    public virtual void StartledFromSleep()
    {

        isSleeping = false;
        //TODO ������ʾ��������ͼ��
        Debug.Log("������");
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
    /// �ı��ⷶΧ
    /// </summary>
    /// <param name="radius"></param>
    public void ChangeDetectRadius(float radius)
    {
        TargetDetector targetDetector = GetComponentInChildren<TargetDetector>();
        targetDetector.ChangeRadius(radius);
    }
    //�¹������

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
        // �ж� direction ���� y �᷽�����߻����ұ�
        if (isDefaultLeft)
        {
            if (enemyAI.FinalMovement.x > 0 && !isFlipped) // �� y �᷽����ұߣ��ҵ�ǰû�з�ת
            {
                // ��ת Sprite
                spriteRenderer.flipX = true;
                isFlipped = true;
            }
            else if (enemyAI.FinalMovement.x < 0 && isFlipped) // �� y �᷽�����ߣ��ҵ�ǰ�Ѿ���ת
            {
                // ȡ����ת
                spriteRenderer.flipX = false;
                isFlipped = false;
            }
            return;
        }
        if (enemyAI.FinalMovement.x > 0 && isFlipped) // �� y �᷽����ұߣ��ҵ�ǰû�з�ת
        {
            // ��ת Sprite
            spriteRenderer.flipX = false;
            isFlipped = false;
        }
        else if (enemyAI.FinalMovement.x < 0 && !isFlipped) // �� y �᷽�����ߣ��ҵ�ǰ�Ѿ���ת
        {
            // ȡ����ת
            spriteRenderer.flipX = true;
            isFlipped = true;
        }
    }
}
