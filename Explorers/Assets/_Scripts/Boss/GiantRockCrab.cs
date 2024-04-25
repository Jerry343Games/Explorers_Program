using BehaviorDesigner.Runtime;
using System.Collections.Generic;
using UnityEngine;

//����з	
public class GiantRockCrab : Singleton<GiantRockCrab>
{
    private BehaviorTree _behaviorTree;

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


    //[Header("����")]


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

    #region ǯ��

    public void PincerStrike()
    {
        //����ǯ����������
    }

    //��Ϊǯ���˺��Ĵ�����Ҫ��֡�����¼� �������Ҫ����ǯ�ӻ������һ֡
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

    #region ����
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
        GameObject target = FindNearestPlayer();
        Vector3 dir = (target.transform.position - transform.position).normalized;

        GameObject acidArea = Instantiate(Resources.Load<GameObject>("Effect/AicdFlow"),transform.position,Quaternion.identity);
        acidArea.transform.LookAt(Vector3.right, Vector3.up);
    }

    #endregion

    #region �̻�

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
