using BehaviorDesigner.Runtime;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����з	
public class GiantRockCrab : Singleton<GiantRockCrab>
{

    public BehaviorTree _firstBehaviorTree;

    public BehaviorTree _secondBehaviorTree;

    

    private Rigidbody _rb;

    private BoxCollider _coll;

    private Animator _anim;

    private Vector3 _dir;

    private float _speed;


    [Header("ʵ��")]
    public GameObject entity;


    [Header("��������")]

    public bool animPlayOver;

    [Header("����")]
    public bool hasDead;

    public int maxHealth;

    [HideInInspector]
    public int _currentHealth;

    public float normalSpeed;

    public float actionChangeInterval;//������Ϊ�ļ��ʱ�䣨����Ҫ��һ����Ϊ�������Ķ���ʱ�� ��֤��������Ž�����һ����Ϊ��

    [HideInInspector]
    public float _currentArmor;

    public float maxArmor;

    public int curingTriggerHealth = 150;//�����̻�����Ҫ��ʧ��Ѫ�� �����Ϊ150

    private List<bool> curingStageFlags;//���Ѫ��ÿ�ο�150����

    private bool isSecondStage;//�Ƿ��ڵڶ��׶�

    private bool hasEnteredSecondStage;

    public LayerMask playerLayer;

    [Header("Ѳ��")]
    public bool isPatrol;

    public List<Transform> patrolPoints = new List<Transform>();

    private int currentPatrolIndex;


    [Header("���")]
    public List<PlayerController> inRangePlayers = new List<PlayerController>();

    [Header("ǯ��")]
    //public float closePlayerSpeed;

    //public Transform strikePoint;

    //public float strikeRadius;

    public int strikeDamage;

    public float strikeForce;

    public GameObject strikeCheckObject;

    //[Header("����")]
    //public float rushSpeed;

    //public int rushDamage;

    //public float rushDuration;

    //public float rushForce;

    [Header("��ʯ")]
    public float stoneSpeed;

    public float stoneDuration;

    public int stoneFlyingDamage;

    public int stoneBoomDamage;

    public float stoneBoomRange;

    public float stoneForce;//��ʯ���ɽ�ɫ������

    public int stoneAmount;

    public Transform stoneSpawnCenterPoint;

    [Header("����")]
    public int acidDamage;

    public float acidCorrodeDuration;//��ʴ���ܵ�ʱ��

    public float acidCorrodeRate;

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
        foreach (var tree in trees)
        {
            if (tree.BehaviorName == "GiantRockCrabBehaviorTree_Stage1")
            {
                _firstBehaviorTree = tree;
            }
            else
            {
                _secondBehaviorTree = tree;
            }
        }

        _rb = entity.GetComponent<Rigidbody>();

        _coll = entity.GetComponent<BoxCollider>();

        _anim = entity.GetComponent<Animator>();

    }

    private void Start()
    {
        _speed = normalSpeed;
        _currentArmor = maxArmor;
        curingStageFlags = new List<bool>() { false, false, false, false };
        deterTimer = deterInterval;
        _currentHealth = maxHealth;

        //FindObjectOfType<UIBossPanel>().ShowPanel();

        _anim.Play("Awake");
    }

    public void StartPatrol()
    {
        _coll.enabled = true;
        //�����һ����
        _firstBehaviorTree.enabled = true;
        currentPatrolIndex = 0;
        isPatrol = true;
    }

    public void StopPatrol()
    {
        isPatrol = true;
    }




    private void Update()
    {
        if (hasDead) return;
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


        if(isPatrol)
        {
            SetMoveDirection((patrolPoints[currentPatrolIndex].position - entity.transform.position).normalized);
            SetMoveSpeed(normalSpeed);

            _anim.SetFloat("Speed", _speed);
            if(Vector3.Distance(patrolPoints[currentPatrolIndex].position, entity.transform.position)<1f)
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
        if(isPatrol)
        {
            entity.transform.Translate(_dir * Time.deltaTime * _speed, Space.World);
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
        //_rb.velocity = _dir * _speed;
    }

    public void TakeDamage(int damage)
    {

        if(_currentArmor >damage)
        {
            _currentArmor -= damage;
            //UI
            return;

        }else if(_currentArmor<=damage && _currentArmor>0)
        {
            _currentArmor = 0;
            //UI
            return;
        }

        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);

        //UI
        if(_currentHealth==0 && !hasDead)
        {
            hasDead = true;
            _firstBehaviorTree.enabled = false;
            _secondBehaviorTree.enabled = false;
            isPatrol = false;
            _coll.enabled = false;
            _anim.Play("Dead");
            FindObjectOfType<UIBossPanel>().HidePanel();
            MusicManager.Instance.PlayBackMusic("Level_2");
        }


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

            MusicManager.Instance.PlayBackMusic("Boss_2");
            //���׶�ǿ��
            strikeDamage += 10;
            strikeForce += 5;

            acidDamage += 10;
            acidCorrodeDuration += 2;
        }
    }

    public GameObject FindNearestPlayer()
    {

        if (inRangePlayers.Count == 0) return null;
        GameObject _target = inRangePlayers[0].gameObject;
        float nearestDis = Vector3.Distance(_target.transform.position, entity.transform.position);
        foreach (var character in inRangePlayers)
        {
            float curDis = Vector3.Distance(character.transform.position, entity.transform.position);
            if (curDis < nearestDis)
            {
                nearestDis = curDis;
                _target = character.gameObject;
            }
        }

        Vector3 dir = (_target.transform.position - entity.transform.position).normalized;

        SetFlip(dir.x >= 0);

        return _target;
    }


    public void SetMoveDirection(Vector3 dir)
    {
        _dir = dir;
        SetFlip(dir.x >= 0);
    }

    public void SetFlip(bool right)
    {
        if(right)
        {
            entity.transform.localScale = new Vector3(2,2, 2);
        }
        else
        {
            entity.transform.localScale = new Vector3(2, 2, -2);
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

    #region ǯ��

    public void PincerStrike()
    {
        StartCoroutine(PincerStrikeAction());
    }

    IEnumerator PincerStrikeAction()
    {
        strikeCheckObject.SetActive(true);
        GameObject target = FindNearestPlayer();
        //ǯ����Ϊ
        _anim.Play("NormalAttack");

        Vector3[] path = new Vector3[]
        {
                entity.transform.position,
                target.transform.position,
        };

        Vector3[] path_2 = new Vector3[]
        {
                target.transform.position,
                new Vector3( target.transform.position.x+entity.transform.localScale.z/*Mathf.Abs(target.transform.position.x+entity.transform.localScale.z)>12?(entity.transform.localScale.z>0?1:-1)*12:target.transform.position.x+entity.transform.localScale.z*/,
                entity.transform.position.y,
                0)
        };

        Sequence s = DOTween.Sequence();
        s.Append(entity.transform.DOPath(path, 1).SetEase(Ease.InExpo));
        s.Append(entity.transform.DOPath(path_2, 1).SetEase(Ease.InOutQuart));
        yield return new WaitForSeconds(2f);
        MusicManager.Instance.PlaySound("Boss���");
        CameraTrace.instance.CameraShake(0.2f, 2f);
        strikeCheckObject.SetActive(false);

    }


    #endregion

    //#region ����
    //public void Rush()
    //{
    //    Collider[] colls =  Physics.OverlapSphere(transform.position, 2f, playerLayer);

    //    if (colls.Length == 0) return;

    //    foreach(var coll in colls)
    //    {
    //        if(coll.gameObject.tag=="Player")
    //        {
    //            coll.gameObject.GetComponent<PlayerController>().TakeDamage(rushDamage);
    //            coll.gameObject.GetComponent<Rigidbody>().AddForce((coll.transform.position-transform.position).normalized * rushForce,ForceMode.Impulse);
    //        }
    //        else if(coll.gameObject.tag == "Enemy")
    //        {
    //            coll.gameObject.GetComponent<Rigidbody>().AddForce((coll.transform.position-transform.position).normalized * rushForce,ForceMode.Impulse);
    //        }
    //    }
    //}
    //#endregion

    #region ��ʯ
    public void SpawnFlyingStones()
    {
        //spawning stones in random position 
        MusicManager.Instance.PlaySound("Boss��ʯ");
        for(int i=0;i<stoneAmount;i++)
        {
            GameObject stone = Instantiate(Resources.Load<GameObject>("Item/FlyingStone"),
                stoneSpawnCenterPoint.position+new Vector3(Random.Range(-20f,20f),0,0), 
                Quaternion.identity);
            stone.transform.localScale *= Random.Range(0.7f, 0.9f);

            stone.GetComponent<Stone>().Init(stoneSpeed, stoneFlyingDamage, stoneForce, stoneDuration, stoneBoomDamage, stoneBoomRange);
        }
    }

    #endregion

    #region ����
    public void SpitAcid()
    {
        //���Ŷ���
        _anim.Play("AcidAttack");
    }

    #endregion

    #region �̻���������

    public void Curing()
    {
        _currentArmor = maxArmor;

        Debug.Log("�̻�");

        float angleIncrement = 360f / curingStoneAmount;

        for (int i = 0; i < curingStoneAmount; i++)
        {
            Debug.Log(1);
            float angle = i * angleIncrement;
            float x = Mathf.Cos(Mathf.Deg2Rad * angle)*6;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle)*6;
            GameObject brokenStone = Instantiate(Resources.Load<GameObject>("Item/BrokenStone"),entity.transform.position + new Vector3(x, y, 0), Quaternion.identity);
            brokenStone.transform.localScale *= 2;
            Vector3 dir = (brokenStone.transform.position - entity.transform.position).normalized;
            brokenStone.GetComponent<BrokenStone>().Init(dir, curingStoneDamage, curingStoneForce, curingStoneSpeed, curingStoneDuration);
        }
    }
    #endregion

    #region ���壨������
    public void Deter()
    {
        Debug.Log("����");

        if (!hasDecreaseSpeedOfPlayers)
        {
            hasDecreaseSpeedOfPlayers = true;
            foreach(var player in PlayerManager.Instance.gamePlayers)
            {
                player.GetComponent<PlayerController>().ChangeSpeed(0.7f);
            }
        }
        Collider[] colls = Physics.OverlapSphere(entity.transform.position, deterHitRange,playerLayer);
        //��Ч
        if (colls.Length == 0) return;
        foreach(var coll in colls)
        {
            Vector3 dir = (coll.transform.position - entity.transform.position).normalized;
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
