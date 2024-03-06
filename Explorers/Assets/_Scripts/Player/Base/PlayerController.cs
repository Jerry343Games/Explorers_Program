using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    public PlayerInputSetting playerInputSetting;
    
    /// <summary>
    /// ��������䵽��Ψһ���к�,�������ּ�ö����
    /// </summary>
    [HideInInspector]
    public int myIndex;

    private Rigidbody _rigidbody;
    
    [Header("�ƶ�")]
    public float speed;
    public float accelerateFactor;//��������ϵ��
    private float _speedFactor;//�ܿ�����ϵ����ͨ���޸����ı��ɫ����
    private Vector2 _inputDir;//���뷽��
    private Vector3 _moveDir;//�ƶ�����
    public float vertigoTime = 0.3f;//��������ѣ�ε�ʱ�䣨���ܲ�����
    private float _vertigoTimer = 0;
    private bool _canMove = true;//�Ƿ����ƶ�

    [Header("����")]
    public int maxArmor;//��ػ�����
    protected int currentArmor;//��ػ�����
    public int restoreAmount;//���λ����޸���
    public float restoreCD;//�޸���ȴ
    private float _restoreTimer;

    [Header("����")]
    public float skillCD;//������ȴ
    private float _skillTimer;
    private bool canUseSkill;

    [Header("����")]
    public WeaponDataSO mainWeapon;//������
    public WeaponDataSO secondaryWeapons;//������
    [HideInInspector]
    public WeaponDataSO currentWeapon;//��ǰʹ�õ�����
    private int _currentMainAmmunition, _currentSecondaryAmmunition;//����������ǰ�ӵ���
    private float _mainAttackTimer, _secondaryAttackTimer;
    private bool canMainAttack, canSecondaryAttack;

    [Header("ͨ��")]
    public int attackRange;
    public int attack;
    public bool hasDead;

    [Header("����")]
    public float DistanceThreshold = 10;//������󳤶�
    protected bool _hasConnected;//�Ƿ�������״̬
    protected ObiRope _obiRope;

    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void PlayerInit()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = transform.parent.GetComponent<PlayerInput>();
        playerInputSetting = transform.parent.GetComponent<PlayerInputSetting>();
        myIndex = _playerInput.playerIndex;
        Debug.Log(transform.name+" Index: "+myIndex);
        EnemyManager.Instance.players.Add(gameObject);
        if (gameObject.CompareTag("Battery")) EnemyManager.Instance.battery = gameObject;

        currentArmor = maxArmor;
        currentWeapon = mainWeapon;
        _currentMainAmmunition = mainWeapon.initAmmunition;
        _currentSecondaryAmmunition = secondaryWeapons.initAmmunition;
        canUseSkill = false;
        hasDead = false;
        _speedFactor = 1;
    }
    
    public void SetRope(ObiRope rope = null)
    {
        _obiRope = rope;
        _hasConnected = true;
    }

    /// <summary>
    /// ͨ����ȡPlayerInputSetting�н��ܵ��ķ��򣬺ϲ�������������ƶ�����_moveDir
    /// </summary>
    public void MovementCombination()
    {
        
        _moveDir=new Vector3(playerInputSetting.inputDir.x, playerInputSetting.inputDir.y,0).normalized;
    }
    
    /// <summary>
    /// ��ɫ�ƶ�����
    /// </summary>
    public void CharacterMove()
    {
        //�ж��Ƿ����ƶ���������ܣ��ͼ�ʱѣ��ʱ�����������޻ظ��ƶ�
        if (!_canMove)
        {
            _vertigoTimer += Time.deltaTime;
            if (_vertigoTimer >= vertigoTime)
            {
                _canMove = true;
                _vertigoTimer = 0;
                _rigidbody.velocity = new(0, 0, 0);
            }
            return;
        }
        MovementCombination();
        transform.Translate(_moveDir * Time.deltaTime * speed * _speedFactor, Space.World);
        
        //���������ж�
        if (playerInputSetting.GetAccelerateButtonDown())
        {
            _speedFactor = accelerateFactor;
        }
        if (playerInputSetting.GetAccelerateButtonRelease())
        {
            _speedFactor = 1;
        }
    }
    
    /// <summary>
    /// ������������
    /// </summary>
    public void ReconnectRope()
    {
        _hasConnected = true;
        float rotationZ = Vector3.Angle((transform.position - SceneManager.Instance.BatteryTransform.position).normalized, Vector3.right) * (transform.position.y < SceneManager.Instance.BatteryTransform.position.y ? -1 : 1);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
        GameObject newRopeHanger = Instantiate(Resources.Load<GameObject>("Hanger"), (transform.position + SceneManager.Instance.BatteryTransform.position) / 2, rotation);
        //���ݱ�׼�����ӳ��� �ı䵱ǰ��scale
        newRopeHanger.transform.localScale = new Vector3(Vector3.Distance(transform.position, SceneManager.Instance.BatteryTransform.position) / 4.2f, 1, 1);
        //���ø�������ʵ�����ӹ���
        newRopeHanger.transform.SetParent(SceneManager.Instance.Slover.transform);
        GameObject rope = newRopeHanger.transform.GetChild(0).gameObject;
        _obiRope = rope.GetComponent<ObiRope>();
        ObiParticleAttachment[] attachment = rope.GetComponents<ObiParticleAttachment>();
        //�����������ߵ�ǣ������
        attachment[0].target = SceneManager.Instance.BatteryTransform;
        attachment[1].target = transform;
        GetComponent<CellBattery>().ChangeConnectState(_hasConnected);
    }

    /// <summary>
    /// �ж����ӳ����Ƿ񳬳���ֵ
    /// </summary>
    public void CheckDistanceToBattery()
    {
        if (Vector3.Distance(SceneManager.Instance.BatteryTransform.position, transform.position) > DistanceThreshold && _hasConnected)
        {
            //Destroy(_obiRope.transform.parent.gameObject, 0.5f);
            ObiParticleAttachment[] attachment = _obiRope.GetComponents<ObiParticleAttachment>();
            //�����������ߵ�ǣ������
            attachment[0].enabled = false;
            attachment[1].enabled = false;

            _hasConnected = false;
            //�ܽ�Ч��
            GetComponent<CellBattery>().ChangeConnectState(_hasConnected);
        }
    }
    
    /// <summary>
    /// ��̬���ݾ���ı����ӳ���
    /// </summary>
    //public void DynamicChangeLengthOfRope()
    //{
    //    if (_obiRope == null) return;
    //    _obiRope.stretchingScale = Vector3.Distance(transform.position, SceneManager.Instance.BatteryTransform.position) / 4f;
    //}

    /// <summary>
    /// ���˷���
    /// </summary>
    /// <param name="damage">�˺���</param>
    public void TakeDamage(int damage)
    {
        if(damage < currentArmor)
        {
            currentArmor -= damage;
        }
        else
        {
            int damageToBattery = damage - currentArmor;
            GetComponent<Battery>().ChangePower(-damageToBattery);
            currentArmor = maxArmor;
        }
    }

    /// <summary>
    /// ���������޸�����
    /// </summary>
    public void RestroeDefence()
    {
        if(_restoreTimer<0)
        {
            _restoreTimer = restoreCD;
            GetComponent<CellBattery>().currentPower -= maxArmor;//������Ӧ�����޸�
            currentArmor += restoreAmount;
        }
        else
        {
            _restoreTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// ���¼���CD
    /// </summary>
    public void UpdatSkillState()
    {
        if (_skillTimer < 0 && !canUseSkill)
        {
            canUseSkill = true;
        }
        else
        {
            _skillTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// ʹ�ü���
    /// </summary>
    public virtual void Skill()
    {
        if(canUseSkill)
        {
            Debug.Log("ʹ�ü�����");
            _skillTimer = skillCD;
            canUseSkill = false;
        }
    }
    /// <summary>
    /// ��ѣ��
    /// </summary>
    public virtual void Vertigo(Vector3 force)
    {
        _canMove = false;
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    /// <summary>
    /// ��������״̬
    /// </summary>
    /// <param name="newState"></param>
    public void SetDeadState(bool newState)
    {
        hasDead = newState;
    }

    /// <summary>
    /// ��������CD
    /// </summary>
    public void UpdateAttackState()
    {
        if(_mainAttackTimer<0)
        {
            canMainAttack = true;
        }
        else
        {
            _mainAttackTimer -= Time.deltaTime;
        }
        if (_secondaryAttackTimer < 0)
        {
            canSecondaryAttack = true;
        }
        else
        {
            _secondaryAttackTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// �л�����
    /// </summary>
    public void ChangeWeapon()
    {
        currentWeapon = (currentWeapon == mainWeapon ? secondaryWeapons : mainWeapon);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void Attack()
    {
        if(currentWeapon==mainWeapon)
        {
            canMainAttack = false;
            _mainAttackTimer = currentWeapon.attackCD;
            MainAttack();
        }
        else if(currentWeapon==secondaryWeapons)
        {
            canSecondaryAttack = false;
            _secondaryAttackTimer = currentWeapon.attackCD;
            SecondaryAttack();
        }
    }

    /// <summary>
    /// �������������� ��������д
    /// </summary>
    public virtual void MainAttack() { }

    /// <summary>
    /// �������������� ��������д
    /// </summary>
    public virtual void SecondaryAttack() { }
}
