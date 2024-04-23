using BehaviorDesigner.Runtime;
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

    //[Header("����")]

    //[Header("�̻�")]

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
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;
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



}
