using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using UnityEngine;

//巨岩蟹	
public class GiantRockCrab : Singleton<GiantRockCrab>
{
    private BehaviorTree _behaviorTree;

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


    //[Header("震慑")]


    protected override void Awake()
    {
        base.Awake();

        _behaviorTree = GetComponent<BehaviorTree>();

        _rb = GetComponent<Rigidbody>();

        _coll = GetComponent<BoxCollider>();

    }

    private void Start()
    {
        _speed = normalSpeed;
        _currentArmor = maxArmor;
        curingStageFlags = new List<bool>() { false, false, false, false };

    }
    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
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
    }

    public GameObject FindNearestPlayer()
    {
        GameObject _target = PlayerManager.Instance.gamePlayers[0];
        float nearestDis = Vector3.Distance(_target.transform.position, GiantRockCrab.Instance.gameObject.transform.position);
        foreach (var character in PlayerManager.Instance.gamePlayers)
        {
            float curDis = Vector3.Distance(character.transform.position, GiantRockCrab.Instance.gameObject.transform.position);
            if (curDis < nearestDis)
            {
                nearestDis = curDis;
                _target = character;
            }
        }
        return _target;
    }

    public void SetMoveDirection(Vector3 dir)
    {
        _dir = dir;
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
        Collider[] colls = Physics.OverlapSphere(strikePoint.transform.position, strikeRadius);

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
        Collider[] colls =  Physics.OverlapBox(_coll.center, _coll.size/2f);

        if (colls.Length == 0) return;

        foreach(var coll in colls)
        {
            if(coll.gameObject.tag=="Player")
            {
                coll.gameObject.GetComponent<PlayerController>().TakeDamage(rushDamage);
                coll.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - coll.transform.position).normalized * rushForce,ForceMode.Impulse);
            }
            else if(coll.gameObject.tag == "Enemy")
            {
                coll.gameObject.GetComponent<Rigidbody>().AddForce((transform.position - coll.transform.position).normalized * rushForce,ForceMode.Impulse);
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
        GameObject target = FindNearestPlayer();
        Vector3 dir = (target.transform.position - transform.position).normalized;

        GameObject acidArea = Instantiate(Resources.Load<GameObject>("Effect/AicdFlow"),transform.position,Quaternion.identity);
        acidArea.transform.LookAt(Vector3.right, Vector3.up);
    }

    #endregion

    #region 固化

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
}
