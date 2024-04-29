using BehaviorDesigner.Runtime;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//巨岩蟹	
public class GiantRockCrab : Singleton<GiantRockCrab>
{

    private BehaviorTree _firstBehaviorTree;

    private BehaviorTree _secondBehaviorTree;


    private Rigidbody _rb;

    private BoxCollider _coll;

    private Vector3 _dir;

    private float _speed;

    [Header("动画属性")]

    public bool animPlayOver;


    

    [Header("属性")]
    public int maxHealth;

    private int _currentHealth;

    public float normalSpeed;

    public float actionChangeInterval;//两种行为的间隔时间（至少要有一个行为的完整的动画时间 保证动画播完才进行下一个行为）

    private float _currentArmor;

    public float maxArmor;

    public int curingTriggerHealth = 150;//触发固化单次要损失的血量 设计中为150

    private List<bool> curingStageFlags;//标记血量每次扣150触发

    private bool isSecondStage;//是否处于第二阶段

    private bool hasEnteredSecondStage;

    public LayerMask playerLayer;

    [Header("巡逻")]
    public bool isPatrol;

    public List<Transform> patrolPoints = new List<Transform>();

    private int currentPatrolIndex;


    [Header("玩家")]
    public List<PlayerController> inRangePlayers = new List<PlayerController>();

    [Header("钳击")]
    public float closePlayerSpeed;

    public Transform strikePoint;

    public float strikeRadius;

    public int strikeDamage;

    public float strikeForce;

    [Header("疾行")]
    public float rushSpeed;

    public int rushDamage;

    public float rushDuration;

    public float rushForce;

    [Header("飞石")]
    public float stoneSpeed;

    public float stoneDuration;

    public int stoneFlyingDamage;

    public int stoneBoomDamage;

    public float stoneBoomRange;

    public float stoneForce;//飞石击飞角色的力量

    [Header("吐酸")]
    public int acidDamage;

    public float acidCorrodeDuration;//腐蚀护盾的时间

    public float acidCorrodeRange;//酸雾范围（圆形半径）

    [Header("固化")]

    public  int curingStoneDamage;

    public int curingStoneAmount=8;

    public float curingStoneForce;

    public float curingStoneSpeed;

    public float curingStoneDuration;


    [Header("震慑")]
    public float deterInterval;

    private bool hasDecreaseSpeedOfPlayers;


    private float deterTimer;

    public float deterHitRange = 7;

    public float deterHitForce;


    protected override void Awake()
    {
        base.Awake();

        BehaviorTree[] trees = GetComponents<BehaviorTree>();
        foreach(var tree in trees)
        {
            if(tree.BehaviorName== "GiantRockCrabBehaviorTree_Stage1")
            {
                _firstBehaviorTree = tree;
            }
            else
            {
                _secondBehaviorTree = tree;
            }
        }

        _rb = GetComponent<Rigidbody>();

        _coll = GetComponent<BoxCollider>();

    }

    private void Start()
    {
        _speed = normalSpeed;
        _currentArmor = maxArmor;
        curingStageFlags = new List<bool>() { false, false, false, false };
        deterTimer = deterInterval;
        _currentHealth = maxHealth;
        currentPatrolIndex = 0;
        isPatrol = true;
        SetMoveDirection((patrolPoints[currentPatrolIndex].position - transform.position).normalized);
        //激活第一个树
        _firstBehaviorTree.Start();
    }

    private void Update()
    {
        //二阶段震慑计时
        if(isSecondStage)
        {
            if(deterTimer<0)
            {
                Deter();
                deterTimer = deterInterval;
            }
            else
            {
                deterTimer -= Time.deltaTime;
            }
        }

        if(isPatrol)
        {
            SetMoveDirection((patrolPoints[currentPatrolIndex].position - transform.position).normalized);
            SetMoveSpeed(normalSpeed);
            if(Vector3.Distance(patrolPoints[currentPatrolIndex].position, transform.position)<1f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            }
        }
        else
        {
            _dir = Vector3.zero;
        }

    }
    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);

        //固化判断
        if (_currentHealth < maxHealth-curingTriggerHealth && !curingStageFlags[0])
        {
            curingStageFlags[0] = true;
            Curing();
        }
        else if(_currentHealth < maxHealth - curingTriggerHealth*2 && !curingStageFlags[1])
        {
            curingStageFlags[1] = true;
            Curing();
        }
        else if (_currentHealth < maxHealth - curingTriggerHealth * 3 && !curingStageFlags[2])
        {
            curingStageFlags[2] = true;
            Curing();
        }
        else if (_currentHealth < maxHealth - curingTriggerHealth * 4 && !curingStageFlags[3])
        {
            curingStageFlags[3] = true;
            Curing();
        }
        //判断是否处于第二阶段 是的话就切换行为树 并且只切换一次
        if(_currentHealth<maxHealth/2 && !hasEnteredSecondStage)
        {
            hasEnteredSecondStage = true;
            isSecondStage = true;
            _firstBehaviorTree.enabled = false;
            _secondBehaviorTree.enabled = true;
        }
    }

    public GameObject FindNearestPlayer()
    {

        if (inRangePlayers.Count == 0) return null;
        GameObject _target = inRangePlayers[0].gameObject;
        float nearestDis = Vector3.Distance(_target.transform.position, transform.position);
        foreach (var character in PlayerManager.Instance.gamePlayers)
        {
            float curDis = Vector3.Distance(character.transform.position, transform.position);
            if (curDis < nearestDis)
            {
                nearestDis = curDis;
                _target = character;
            }
        }

        Vector3 dir = (_target.transform.position - transform.position).normalized;

        SetFlip(dir.x >= 0);

        return _target;
    }

    public void SetMoveDirection(Vector3 dir)
    {
        _dir = dir;
    }

    public void SetFlip(bool right)
    {
        if(right)
        {
            transform.localScale = new Vector3(2, 2, 2);
        }
        else
        {
            transform.localScale = new Vector3(2, 2, -2);
        }
    }

    public void SetMoveSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetAnimPlayOver()
    {
        animPlayOver = true;
    }

    #region 钳击

    public void PincerStrike()
    {
        //播放钳击动画即可
    }

    //因为钳击伤害的触发点要用帧动画事件 这个方法要挂在钳子击打的那一帧
    public void PincerStrikeAction()
    {
        Collider[] colls = Physics.OverlapSphere(strikePoint.transform.position, strikeRadius, playerLayer);

        if (colls.Length == 0) return;

        foreach(var coll in colls)
        {
            coll.gameObject.GetComponent<PlayerController>().TakeDamage(strikeDamage);
            coll.gameObject.GetComponent<PlayerController>().Vertigo(
                new Vector3(Random.Range(-1, 1) * strikeForce, Random.Range(-1, 1) * strikeForce, 0),
                ForceMode.Impulse, 2f); 
        }
    }

    #endregion

    #region 疾行
    public void Rush()
    {
        Collider[] colls =  Physics.OverlapSphere(transform.position, 2f, playerLayer);

        if (colls.Length == 0) return;

        foreach(var coll in colls)
        {
            if(coll.gameObject.tag=="Player")
            {
                coll.gameObject.GetComponent<PlayerController>().TakeDamage(rushDamage);
                coll.gameObject.GetComponent<Rigidbody>().AddForce((coll.transform.position-transform.position).normalized * rushForce,ForceMode.Impulse);
            }
            else if(coll.gameObject.tag == "Enemy")
            {
                coll.gameObject.GetComponent<Rigidbody>().AddForce((coll.transform.position-transform.position).normalized * rushForce,ForceMode.Impulse);
            }
        }
    }
    #endregion

    #region 飞石
    public void SpawnFlyingStones()
    {
        //如果有扔石头的动画 要在那一帧生成 并且生成到手的位置
        GameObject stone = Instantiate(Resources.Load<GameObject>("Item/FlyingStone"), transform.position, Quaternion.identity);

        GameObject target = FindNearestPlayer();

        Vector3 dir = (target.transform.position - transform.position).normalized;

        stone.GetComponent<Stone>().Init(dir,stoneSpeed, stoneFlyingDamage, stoneForce, stoneDuration, stoneBoomDamage, stoneBoomRange);
    }

    #endregion

    #region 吐酸
    public void SpitAcid()
    {
        //生成酸雾区域

        GameObject acidArea = Instantiate(Resources.Load<GameObject>("Effect/AicdFlow"), transform.position + new Vector3(transform.lossyScale.x,0, 0), Quaternion.Euler(0,90,0));
        acidArea.transform.SetParent(transform);
        Destroy(acidArea, 1f);
    }

    #endregion

    #region 固化（被动）

    public void Curing()
    {
        _currentArmor = maxArmor;


        float angleIncrement = 360f / curingStoneAmount;

        for (int i = 0; i < curingStoneAmount; i++)
        {
            Debug.Log(1);
            float angle = i * angleIncrement;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle)*3;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle)*3;
            GameObject brokenStone = Instantiate(Resources.Load<GameObject>("Item/BrokenStone"), transform.position + new Vector3(x, y, 0), Quaternion.identity);
            Vector3 dir = (brokenStone.transform.position - transform.position).normalized;
            brokenStone.GetComponent<BrokenStone>().Init(dir, curingStoneDamage, curingStoneForce, curingStoneSpeed, curingStoneDuration);
        }
    }
    #endregion

    #region 震慑（被动）
    public void Deter()
    {
        if(!hasDecreaseSpeedOfPlayers)
        {
            hasDecreaseSpeedOfPlayers = true;
            foreach(var player in PlayerManager.Instance.gamePlayers)
            {
                player.GetComponent<PlayerController>().ChangeSpeed(0.7f);
            }
        }
        Collider[] colls = Physics.OverlapSphere(transform.position, deterHitRange,playerLayer);
        //特效
        if (colls.Length == 0) return;
        foreach(var coll in colls)
        {
            Vector3 dir = (coll.transform.position - transform.position).normalized;
            coll.GetComponent<Rigidbody>().AddForce(dir * deterHitForce, ForceMode.Impulse);
        }
    }

    #endregion

    public virtual void Paralysis(float continuedTime)
    {
        StartCoroutine(ParalysiseEffect(continuedTime));
    }

    IEnumerator ParalysiseEffect(float continuedTime)
    {
        _speed *= 0.1f;
        yield return new WaitForSeconds(continuedTime);
        _speed /= 0.1f;
    }
}
