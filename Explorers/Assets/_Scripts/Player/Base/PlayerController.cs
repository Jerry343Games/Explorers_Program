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
    public float vertigoTime = 0.3f;//被攻击后眩晕的时间（不能操作）
    private float _vertigoTimer = 0;
    private bool _canMove = true;//是否能移动

    [Header("护盾")]
    public int maxArmor;//电池护盾量
    protected int currentArmor;//电池护盾量
    public int restoreAmount;//单次护盾修复量
    public float restoreCD;//修复冷却
    private float _restoreTimer;

    [Header("功能")]
    public float skillCD;//功能冷却
    private float _skillTimer;
    private bool canUseSkill;

    [Header("武器")]
    public WeaponDataSO mainWeapon;//主武器
    public WeaponDataSO secondaryWeapons;//副武器
    [HideInInspector]
    public WeaponDataSO currentWeapon;//当前使用的武器
    private int _currentMainAmmunition, _currentSecondaryAmmunition;//主副武器当前子弹数
    private float _mainAttackTimer, _secondaryAttackTimer;
    private bool canMainAttack, canSecondaryAttack;

    [Header("通用")]
    public int attackRange;
    public int attack;
    public bool hasDead;

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
    /// 通过获取PlayerInputSetting中接受到的方向，合并输入向量获得移动方向_moveDir
    /// </summary>
    public void MovementCombination()
    {
        
        _moveDir=new Vector3(playerInputSetting.inputDir.x, playerInputSetting.inputDir.y,0).normalized;
    }
    
    /// <summary>
    /// 角色移动方法
    /// </summary>
    public void CharacterMove()
    {
        //判断是否不能移动，如果不能，就计时眩晕时长，到达上限回复移动
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
        
        //主动加速判断
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
            //Destroy(_obiRope.transform.parent.gameObject, 0.5f);
            ObiParticleAttachment[] attachment = _obiRope.GetComponents<ObiParticleAttachment>();
            //设置绳子两边的牵引对象
            attachment[0].enabled = false;
            attachment[1].enabled = false;

            _hasConnected = false;
            //溶解效果
            GetComponent<CellBattery>().ChangeConnectState(_hasConnected);
        }
    }
    
    /// <summary>
    /// 动态根据距离改变绳子长度
    /// </summary>
    //public void DynamicChangeLengthOfRope()
    //{
    //    if (_obiRope == null) return;
    //    _obiRope.stretchingScale = Vector3.Distance(transform.position, SceneManager.Instance.BatteryTransform.position) / 4f;
    //}

    /// <summary>
    /// 受伤方法
    /// </summary>
    /// <param name="damage">伤害量</param>
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
    /// 电量护盾修复方法
    /// </summary>
    public void RestroeDefence()
    {
        if(_restoreTimer<0)
        {
            _restoreTimer = restoreCD;
            GetComponent<CellBattery>().currentPower -= maxArmor;//消耗相应电量修复
            currentArmor += restoreAmount;
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
    /// <summary>
    /// 被眩晕
    /// </summary>
    public virtual void Vertigo(Vector3 force)
    {
        _canMove = false;
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    /// <summary>
    /// 设置死亡状态
    /// </summary>
    /// <param name="newState"></param>
    public void SetDeadState(bool newState)
    {
        hasDead = newState;
    }

    /// <summary>
    /// 更新武器CD
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
    /// 切换武器
    /// </summary>
    public void ChangeWeapon()
    {
        currentWeapon = (currentWeapon == mainWeapon ? secondaryWeapons : mainWeapon);
    }

    /// <summary>
    /// 攻击方法
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
    /// 主武器攻击方法 请子类重写
    /// </summary>
    public virtual void MainAttack() { }

    /// <summary>
    /// 副武器攻击方法 请子类重写
    /// </summary>
    public virtual void SecondaryAttack() { }
}
