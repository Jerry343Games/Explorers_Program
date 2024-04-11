using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    public PlayerInputSetting playerInputSetting;
    
    //动画相关
    public GameObject playerSprite;
    [HideInInspector]
    public Animator animator;
    private SpriteRenderer _spriteRenderer;
    
    //气泡
    [HideInInspector]
    public UIBubblePanel bubblePanel;
    
    /// <summary>
    /// 玩家所分配到的唯一序列号,默认类型区分见枚举类
    /// </summary>
    [HideInInspector]
    public int myIndex;

    private Rigidbody _rigidbody;
    
    [Header("移动")]
    public float speed;
    public float accelerateFactor;//加速移速设置系数
    private float _speedFactor;//加速移动计算系数
    private float _outSpeedFactor;//外界移速系数
    private Vector2 _inputDir;//输入方向
    private Vector3 _moveDir;//移动方向
    public float vertigoTime = 0.3f;//被攻击后眩晕的时间（不能操作）
    private float _vertigoTimer = 0;
    private bool _canMove = true;//是否能移动
    public bool isMoveReverse = false;
    public bool _isAniLeft = false;//动画向左
    [Header("护盾")]
    public int maxArmor;//电池护盾量
    [HideInInspector]
    public int currentArmor;//电池护盾量
    public int restoreAmount;//单次护盾修复量
    public float restoreCD;//修复冷却
    private float _restoreTimer;
    private float lastHurtTimer;
    public float timeToRepairArmor;//脱战多久修复护盾


    [Header("武器")]
    public WeaponDataSO mainWeapon;//主武器
    public WeaponDataSO secondaryWeapons;//副武器
    private int _currentMainAmmunition, _currentSecondaryAmmunition;//主副武器当前子弹数
    public float _mainAttackTimer, _secondaryAttackTimer;
    public bool canMainAttack, canSecondaryAttack;
    [HideInInspector]
    public Vector3 myAimPos;//瞄准方向

    [Header("通用")]
    public bool hasDead;
    public OptionalFeature feature;//选择的技能
    [HideInInspector]
    public float _featureCDTimer;
    [HideInInspector]
    public float featureCD;
    [HideInInspector]
    public bool canUseFeature;

    [Header("绳子")]
    public float DistanceThreshold = 10;//绳子最大长度
    protected bool _hasConnected;//是否处于连接状态
    protected ObiRope _obiRope;

    [Header("开采资源")]
    public bool isDigging;
    protected Resource _curDigRes;

    [Header("道具")]
    public Item item;
    [HideInInspector]
    public Vector3 mouseWorldPS => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));




    /// <summary>
    /// 初始化方法
    /// </summary>
    public void PlayerInit()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerInput = transform.parent.GetComponent<PlayerInput>();
        playerInputSetting = transform.parent.GetComponent<PlayerInputSetting>();
        Debug.Log("Clone: "+transform.name+" / Index: "+myIndex);
        PlayerManager.Instance.gamePlayers.Add(gameObject);
        //if (gameObject.CompareTag("Battery")) EnemyManager.Instance.battery = gameObject;
        myIndex = playerInput.playerIndex;
        currentArmor = maxArmor;
        lastHurtTimer = timeToRepairArmor;
        _currentMainAmmunition = mainWeapon.initAmmunition;
        _currentSecondaryAmmunition = secondaryWeapons.initAmmunition;
        hasDead = false;
        isDigging = false;
        _speedFactor = 1;
        _outSpeedFactor = 1;
        
        //动画相关
        animator = playerSprite.GetComponent<Animator>();
        _spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        
        ////获得自选功能
        //feature = playerInputSetting.feature;
        Debug.Log(playerInputSetting.feature);
        
        bubblePanel = GameObject.Find("BubblePanel").GetComponents<UIBubblePanel>()[0];
        
        //选择关卡时使用UI键位映射，反之则用Player映射
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name=="SelectScene")
        {
            playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
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
        if (isDigging) return;
        MovementCombination();
        if (!isMoveReverse)
        {
            transform.Translate(_moveDir * Time.deltaTime * speed * _speedFactor * _outSpeedFactor, Space.World);
        }
        else
        {
            transform.Translate(-_moveDir * Time.deltaTime * speed * _speedFactor * _outSpeedFactor, Space.World);
        }
        
        
        //主动加速判断
        if (playerInputSetting.GetAccelerateButtonDown())
        {
            _speedFactor = accelerateFactor;
        }
        else
        {
            _speedFactor = 1;
        }
    }
    public void MoveReverse(float reversedTime)
    {
        isMoveReverse = true;
        Invoke("MoveReverseReturnNormal", reversedTime);
    }
    private void MoveReverseReturnNormal()
    {
        isMoveReverse = false;
    }
    /// <summary>
    /// 动画控制
    /// </summary>
    /// <param name="run">移动动画名</param>
    /// <param name="idle">待机动画名</param>
    public void MoveAnimationControl(CharacterAnimation run,CharacterAnimation idle)
    {
        if (playerInputSetting.inputDir.x != 0)
        {
            if (playerInputSetting.inputDir.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            //过渡动画、切换法线
            animator.CrossFade(run.ToString(),0);
            _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetTextureByAnimationName(run));
        }
        else
        {
            animator.CrossFade(idle.ToString(),0);
            _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetTextureByAnimationName(idle));
        }
    }
    
    public void MoveAnimationControlTest(CharacterAnimation run_left,CharacterAnimation run_right,CharacterAnimation idle_left,CharacterAnimation idle_right)
    {
        if (playerInputSetting.inputDir.x != 0)
        {
            if (playerInputSetting.inputDir.x < 0)
            {
                _isAniLeft = true;
                animator.CrossFade(run_left.ToString(), 0f);
                _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetTextureByAnimationName(run_left));
            }
            else
            {
                _isAniLeft = false;
                animator.CrossFade(run_right.ToString(), 0f);
                _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetTextureByAnimationName(run_right));
            }
            
        }
        else
        {
            if (_isAniLeft)
            {
                animator.CrossFade(idle_left.ToString(),0);
                _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetTextureByAnimationName(idle_left));
            }
            else
            {
                animator.CrossFade(idle_right.ToString(),0);
                _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetTextureByAnimationName(idle_right));
            }
            
        }
    }
    
    
    /// <summary>
    /// 绳子重连方法
    /// </summary>
    public void ReconnectRope()
    {
        _hasConnected = true;
        GetComponent<CellBattery>().ChangeConnectState(_hasConnected);
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

        MusicManager.Instance.PlaySound("连接电缆");

    }

    /// <summary>
    /// 判断绳子长度是否超出阈值
    /// </summary>
    public void CheckDistanceToBattery()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SelectScene") return;
        if (!SceneManager.Instance.BatteryTransform) return;
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
            MusicManager.Instance.PlaySound("断开电缆");

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
    public virtual void TakeDamage(int damage)
    {
        if (hasDead) return;
        Debug.Log(name);
        if(isDigging)
        {
            isDigging = false;//打断状态
            _curDigRes.GetComponent<Resource>().beDigging = false;
            _curDigRes.SetDiager(null);
            _curDigRes = null;
        }
        lastHurtTimer = 0;
        if (damage < currentArmor)
        {
            currentArmor -= damage;
        }
        else
        {
            MusicManager.Instance.PlaySound("电能护盾破碎");

            int damageToBattery = damage - currentArmor;
            GetComponent<Battery>().ChangePower(-damageToBattery);
            currentArmor = maxArmor;
        }
        if (damage>0)
        {
            CameraTrace.instance.CameraShake(1f,0.2f);
        }
    }

    /// <summary>
    /// 电量护盾修复方法
    /// </summary>
    public void RestroeDefence()
    {
        //如果进入到腐蚀状态，那就不回甲了
        if (isDefenceDowning)
        {
            if (defenceDownTimer < defenceDownTime)
            {
                defenceDownTimer += Time.deltaTime;
                //掉甲
                currentArmor -= (int)(downRate * Time.deltaTime);
                currentArmor = Math.Max(currentArmor, 0);
                Debug.Log("腐蚀状态，当前护甲值：" + currentArmor);
            }
            else
            {
                isDefenceDowning = false;
                defenceDownTimer = 0;

            }

            return;
        }
        if(lastHurtTimer<timeToRepairArmor)
        {
            lastHurtTimer += Time.deltaTime;
            return;
        }
        if(_restoreTimer<0)
        {
            _restoreTimer = restoreCD;
            if(currentArmor<maxArmor)
            {
                if(_hasConnected)
                {
                    SceneManager.Instance.BatteryTransform.GetComponent<MainBattery>().ChangePower(-restoreAmount);//消耗主电池相应电量修复
                }
                else
                {
                    GetComponent<CellBattery>().ChangePower(-restoreAmount);
                }
                currentArmor = Mathf.Min(maxArmor, currentArmor+restoreAmount);
            }
        }
        else
        {
            _restoreTimer -= Time.deltaTime;
        }
    }
    private float defenceDownTimer = 0f;
    private float defenceDownTime;
    private bool isDefenceDowning = false;
    private float downRate;
    /// <summary>
    /// 缓慢掉甲，掉到0或者倒计时结束
    /// </summary>
    public void DefenceDown(float downTime,float downRate)
    {
        defenceDownTime = downTime;
        isDefenceDowning = true;
        this.downRate = downRate;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void UpdateFeatureState()
    {
        if(!canUseFeature)
        {
            _featureCDTimer -= Time.deltaTime;
            if(_featureCDTimer<0)
            {
                canUseFeature = true;
            }
        }
        
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    //public virtual bool Skill()
    //{
    //    if(canUseSkill)
    //    {
    //        Debug.Log("使用技能了");
    //        _skillTimer = skillCD;
    //        canUseSkill = false;
    //        return true;
    //    }
    //    return false;
    //}
    /// <summary>
    /// 被眩晕
    /// </summary>
    public virtual void Vertigo(Vector3 force,ForceMode forceMode=ForceMode.Impulse,float vertigoTime = 0.3f)
    {
        this.vertigoTime = vertigoTime;
        _canMove = false;
        _rigidbody.AddForce(force, forceMode);
    }

    /// <summary>
    /// 设置死亡状态
    /// </summary>
    /// <param name="newState"></param>
    public void SetDeadState(bool newState)
    {
        hasDead = newState;
        if(hasDead)
        {
            MusicManager.Instance.PlaySound("角色电力耗尽");
        }
    }

    /// <summary>
    /// 更新武器CD
    /// </summary>
    public virtual void UpdateAttackState()
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
    /// 使weapon的前向始终朝向_aimDirection
    /// </summary>
    /// <param name="weapon">武器</param>
    public void Aim(GameObject weapon)
    {
        if (!playerInputSetting.isStick)
        {
            myAimPos = new Vector3(playerInputSetting.aimPos.x, playerInputSetting.aimPos.y, 0);
            weapon.transform.LookAt(myAimPos);
        }
        else
        {
            if (playerInputSetting.aimPos!=Vector2.zero)
            {
                myAimPos = new Vector3(playerInputSetting.aimPos.x, playerInputSetting.aimPos.y, 0)+transform.position;
                weapon.transform.LookAt(myAimPos);
            }
        }
    }
    
    /// <summary>
    /// 主武器攻击方法 请子类重写
    /// </summary>
    public virtual bool MainAttack()
    {
        if (isDigging) return false;
        if (!canMainAttack) return false;
        canMainAttack = false;
        _mainAttackTimer = mainWeapon.attackCD;
        return true;
    }

    /// <summary>
    /// 副武器攻击方法 请子类重写
    /// </summary>
    public virtual bool SecondaryAttack() 
    {
        if (isDigging) return false;
        if (!canSecondaryAttack) return false;
        canSecondaryAttack = false;
        _secondaryAttackTimer = secondaryWeapons.attackCD;
        return true;
    }

    /// <summary>
    /// 按交互物体创建一个气泡UI，记得在退出时删除
    /// </summary>
    /// <param name="other"></param>
    public void CreatBubbleUI(GameObject other)
    {
        switch (other.tag)
        {
            case "Resource":
                if (!hasDead)
                {
                    BubbleInfo resInfo = new BubbleInfo
                    {
                        Type = BubbleType.ResourceCollectionBubble,
                        Obj1 = gameObject,
                        Obj2 = other.gameObject,
                        Content = "采集"
                    };
                    bubblePanel.CreateBubble(resInfo);
                }
                break;
            case "ReconnectArea":
                if (!_hasConnected&&!hasDead){
                BubbleInfo recInfo = new BubbleInfo
                {
                    Type = BubbleType.ReconnectCableBubble,
                    Obj1 = gameObject,
                    Obj2= other.gameObject,
                    Content = "重新连接"
                };
                bubblePanel.CreateBubble(recInfo);
                }
                break;
            case "Chest":
                if (!hasDead)
                {
                    BubbleInfo recInfo = new BubbleInfo
                    {
                        Type = BubbleType.ChestOpenBubble,
                        Obj1 = gameObject,
                        Obj2 = other.gameObject,
                        Content = "打开"
                    };
                    bubblePanel.CreateBubble(recInfo);
                }


                break;
            default:
                break;
        }
    }


    public void DiggingOver()
    {
        isDigging = false;
        _curDigRes = null;
        bubblePanel.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
    }
    
    public void CheckKeys()

    {
        if (playerInputSetting.GetAccelerateButtonDown())
        {
            Debug.Log(name+" Press Accelerate");
        }
        if (playerInputSetting.GetCableButtonDown())
        {
            Debug.Log(name+" Press Cable");
        }
        if (playerInputSetting.GetAttackButtonDown())
        {
            Debug.Log(name+" Press Attack");
        }
        if (playerInputSetting.GetInteractButtonDown())
        {
            Debug.Log(name+ " Press Interact");
        }
        if (playerInputSetting.GetOptionalFeatureDown())
        {
            Debug.Log(name+ "Press Optional");
        }
        if (playerInputSetting.GetAttackSecondaryDown())
        {
            Debug.Log(name+ "Press Attack Secondary");
        }
        if (playerInputSetting.GetUseItem())
        {
            Debug.Log(name + "Press Use Item");
        }
    }
    public void UseItem()
    {
        if (item != null && playerInputSetting.GetUseItem())
        {
            Debug.Log("use" + item.name);
            item.Use(gameObject);
            
        }
    }
    
    public virtual void SetFeatureCD()
    {

    }
}
