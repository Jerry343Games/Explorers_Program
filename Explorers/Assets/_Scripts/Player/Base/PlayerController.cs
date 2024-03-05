using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerInputSetting _playerInputSetting;
    
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

    [Header("����")]
    public int maxDefence;//��ػ�����
    protected int currentDefence;//��ػ�����
    public int restoreAmount;//���λ����޸���
    public float restoreCD;//�޸���ȴ
    private float _restoreTimer;

    [Header("����")]
    public float skillCD;//������ȴ
    private float _skillTimer;
    private bool canUseSkill;

    [Header("ͨ��")]
    public int attack;//������
    public float attackRange;//������Χ
    public float attackCD;//������ȴ

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
        _playerInputSetting = transform.parent.GetComponent<PlayerInputSetting>();
        myIndex = _playerInput.playerIndex;
        Debug.Log(transform.name+" Index: "+myIndex);
        EnemyManager.Instance.players.Add(gameObject);
        if (gameObject.CompareTag("Battery")) EnemyManager.Instance.battery = gameObject;

        currentDefence = maxDefence;
        _restoreTimer = restoreCD;
        _skillTimer = skillCD;
        canUseSkill = false;
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
        _moveDir=new Vector3(_playerInputSetting.inputDir.x, _playerInputSetting.inputDir.y,0).normalized;
    }
    
    /// <summary>
    /// ��ɫ�ƶ�����
    /// </summary>
    public void CharacterMove()
    {
        MovementCombination();
        transform.Translate(_moveDir * Time.deltaTime * speed * _speedFactor, Space.World);
        
        //���������ж�
        if (_playerInputSetting.GetAccelerateButtonDown())
        {
            _speedFactor = accelerateFactor;
        }
        if (_playerInputSetting.GetAccelerateButtonRelease())
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
            Destroy(_obiRope.transform.parent.gameObject, 0.5f);
            _hasConnected = false;
            GetComponent<CellBattery>().ChangeConnectState(_hasConnected);
        }
    }
    
    /// <summary>
    /// ��̬���ݾ���ı����ӳ���
    /// </summary>
    public void DynamicChangeLengthOfRope()
    {
        if (_obiRope == null) return;
        _obiRope.stretchingScale = Vector3.Distance(transform.position, SceneManager.Instance.BatteryTransform.position) / 4f;
    }

    /// <summary>
    /// ���˷���
    /// </summary>
    /// <param name="damage">�˺���</param>
    public void TakeDamage(int damage)
    {
        if(damage < currentDefence)
        {
            currentDefence -= damage;
        }
        else
        {
            int damageToBattery = damage - currentDefence;
            currentDefence = maxDefence;
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
            GetComponent<CellBattery>().currentPower -= maxDefence;//������Ӧ�����޸�
            currentDefence += restoreAmount;
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
}
