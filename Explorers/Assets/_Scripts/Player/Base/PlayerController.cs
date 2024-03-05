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
    /// 玩家所分配到的唯一序列号,类型区分见枚举类
    /// </summary>
    [HideInInspector]
    public int myIndex;

    private Rigidbody _rigidbody;
    
    [Header("移动")]
    public float speed;
    public float accelerateFactor;//加速移速系数
    private float _speedFactor;//总控移速系数，通过修改它改变角色移速
    private Vector2 _inputDir;//输入方向
    private Vector3 _moveDir;//移动方向

    [Header("护盾")]
    public int maxDefence;//电池护盾量
    protected int currentDefence;//电池护盾量
    public int restoreAmount;//单次护盾修复量
    public float restoreCD;//修复冷却
    private float _restoreTimer;

    [Header("功能")]
    public float skillCD;//功能冷却
    private float _skillTimer;
    private bool canUseSkill;

    [Header("通用")]
    public int attack;//攻击力
    public float attackRange;//攻击范围
    public float attackCD;//攻击冷却

    [Header("绳子")]
    public float DistanceThreshold = 10;//绳子最大长度
    protected bool _hasConnected;//是否处于连接状态
    protected ObiRope _obiRope;

    /// <summary>
    /// 初始化方法
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
    /// 通过获取PlayerInputSetting中接受到的方向，合并输入向量获得移动方向_moveDir
    /// </summary>
    public void MovementCombination()
    {
        _moveDir=new Vector3(_playerInputSetting.inputDir.x, _playerInputSetting.inputDir.y,0).normalized;
    }
    
    /// <summary>
    /// 角色移动方法
    /// </summary>
    public void CharacterMove()
    {
        MovementCombination();
        transform.Translate(_moveDir * Time.deltaTime * speed * _speedFactor, Space.World);
        
        //主动加速判断
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
    /// 绳子重连方法
    /// </summary>
    public void ReconnectRope()
    {
        _hasConnected = true;
        float rotationZ = Vector3.Angle((transform.position - SceneManager.Instance.BatteryTransform.position).normalized, Vector3.right) * (transform.position.y < SceneManager.Instance.BatteryTransform.position.y ? -1 : 1);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
        GameObject newRopeHanger = Instantiate(Resources.Load<GameObject>("Hanger"), (transform.position + SceneManager.Instance.BatteryTransform.position) / 2, rotation);
        //根据标准的绳子长度 改变当前的scale
        newRopeHanger.transform.localScale = new Vector3(Vector3.Distance(transform.position, SceneManager.Instance.BatteryTransform.position) / 4.2f, 1, 1);
        //设置父物体以实现绳子功能
        newRopeHanger.transform.SetParent(SceneManager.Instance.Slover.transform);
        GameObject rope = newRopeHanger.transform.GetChild(0).gameObject;
        _obiRope = rope.GetComponent<ObiRope>();
        ObiParticleAttachment[] attachment = rope.GetComponents<ObiParticleAttachment>();
        //设置绳子两边的牵引对象
        attachment[0].target = SceneManager.Instance.BatteryTransform;
        attachment[1].target = transform;
        GetComponent<CellBattery>().ChangeConnectState(_hasConnected);
    }

    /// <summary>
    /// 判断绳子长度是否超出阈值
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
    /// 动态根据距离改变绳子长度
    /// </summary>
    public void DynamicChangeLengthOfRope()
    {
        if (_obiRope == null) return;
        _obiRope.stretchingScale = Vector3.Distance(transform.position, SceneManager.Instance.BatteryTransform.position) / 4f;
    }

    /// <summary>
    /// 受伤方法
    /// </summary>
    /// <param name="damage">伤害量</param>
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
    /// 电量护盾修复方法
    /// </summary>
    public void RestroeDefence()
    {
        if(_restoreTimer<0)
        {
            _restoreTimer = restoreCD;
            GetComponent<CellBattery>().currentPower -= maxDefence;//消耗相应电量修复
            currentDefence += restoreAmount;
        }
        else
        {
            _restoreTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 更新技能CD
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
    /// 使用技能
    /// </summary>
    public virtual void Skill()
    {
        if(canUseSkill)
        {
            Debug.Log("使用技能了");
            _skillTimer = skillCD;
            canUseSkill = false;
        }
    }
}
