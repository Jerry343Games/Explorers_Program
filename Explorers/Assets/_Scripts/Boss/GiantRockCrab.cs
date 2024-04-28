using BehaviorDesigner.Runtime;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����з	
public class GiantRockCrab : Singleton<GiantRockCrab>
{

    private BehaviorTree _firstBehaviorTree;

    private BehaviorTree _secondBehaviorTree;


    private Rigidbody _rb;

    private BoxCollider _coll;

    private Vector3 _dir;

    private float _speed;

    [Header("��������")]

    public bool animPlayOver;

    [Header("����")]
    public int maxHealth;

    private int _currentHealth;

    public float normalSpeed;

    public float actionChangeInterval;//������Ϊ�ļ��ʱ�䣨����Ҫ��һ����Ϊ�������Ķ���ʱ�� ��֤��������Ž�����һ����Ϊ��

    private float _currentArmor;

    public float maxArmor;

    public int curingTriggerHealth = 150;//�����̻�����Ҫ��ʧ��Ѫ�� �����Ϊ150

    private List<bool> curingStageFlags;//���Ѫ��ÿ�ο�150����

    private bool isSecondStage;//�Ƿ��ڵڶ��׶�

    private bool hasEnteredSecondStage;

    public LayerMask playerLayer;

    [Header("ǯ��")]
    public float closePlayerSpeed;

    public Transform strikePoint;

    public float strikeRadius;

    public int strikeDamage;

    public float strikeForce;

    [Header("����")]
    public float rushSpeed;

    public int rushDamage;

    public float rushDuration;

    public float rushForce;

    [Header("��ʯ")]
    public float stoneSpeed;

    public float stoneDuration;

    public int stoneFlyingDamage;

    public int stoneBoomDamage;

    public float stoneBoomRange;

    public float stoneForce;//��ʯ���ɽ�ɫ������

    [Header("����")]
    public int acidDamage;

    public float acidCorrodeDuration;//��ʴ���ܵ�ʱ��

    public float acidCorrodeRange;//����Χ��Բ�ΰ뾶��

    [Header("�̻�")]

    public  int curingStoneDamage;

    public int curingStoneAmount=8;

    public float curingStoneForce;

    public float curingStoneSpeed;

    public float curingStoneDuration;


    [Header("����")]
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
        //�����һ����
        _firstBehaviorTree.Start();
    }

    private void Update()
    {
        //���׶������ʱ
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
        //FindNearestPlayer();

    }
    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);

        //�̻��ж�
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
        //�ж��Ƿ��ڵڶ��׶� �ǵĻ����л���Ϊ�� ����ֻ�л�һ��
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
        GameObject _target = PlayerManager.Instance.gamePlayers[0];
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
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (dir.x >= 0)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            transform.DORotate(new Vector3(-angle, 90, 0f), 1f);
        }
        else
        {
            transform.localScale = new Vector3(1.5f, 1.5f, -1.5f);
            transform.DORotate(new Vector3(-(angle + 180f), 90, 0),1f);

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

    #region ǯ��

    public void PincerStrike()
    {
        //����ǯ����������
    }

    //��Ϊǯ���˺��Ĵ�����Ҫ��֡�����¼� �������Ҫ����ǯ�ӻ������һ֡
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

    #region ����
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

    #region ��ʯ
    public void SpawnFlyingStones()
    {
        //�������ʯͷ�Ķ��� Ҫ����һ֡���� �������ɵ��ֵ�λ��
        GameObject stone = Instantiate(Resources.Load<GameObject>("Item/FlyingStone"), transform.position, Quaternion.identity);

        GameObject target = FindNearestPlayer();

        Vector3 dir = (target.transform.position - transform.position).normalized;

        stone.GetComponent<Stone>().Init(dir,stoneSpeed, stoneFlyingDamage, stoneForce, stoneDuration, stoneBoomDamage, stoneBoomRange);
    }

    #endregion

    #region ����
    public void SpitAcid()
    {
        //������������

        GameObject acidArea = Instantiate(Resources.Load<GameObject>("Effect/AicdFlow"), transform.position + new Vector3(transform.lossyScale.x,0, 0), Quaternion.Euler(0,90,0));
        acidArea.transform.SetParent(transform);
        Destroy(acidArea, 1f);
    }

    #endregion

    #region �̻���������

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

    #region ���壨������
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
        //��Ч
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
