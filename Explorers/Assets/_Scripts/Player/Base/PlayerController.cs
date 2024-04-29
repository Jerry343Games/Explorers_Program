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
    
    //�������
    public GameObject playerSprite;
    [HideInInspector]
    public Animator animator;
    private SpriteRenderer _spriteRenderer;
    
    //����
    //[HideInInspector]
    //public UIBubblePanel bubblePanel;
    
    /// <summary>
    /// ��������䵽��Ψһ���к�,Ĭ���������ּ�ö����
    /// </summary>
    [HideInInspector]
    public int myIndex;

    private Rigidbody _rigidbody;
    
    [Header("�ƶ�")]
    public float speed;
    public float accelerateFactor;//������������ϵ��
    private float _speedFactor;//�����ƶ�����ϵ��
    private float _outSpeedFactor;//�������ϵ��
    private Vector2 _inputDir;//���뷽��
    private Vector3 _moveDir;//�ƶ�����
    public float vertigoTime = 0.3f;//��������ѣ�ε�ʱ�䣨���ܲ�����
    private float _vertigoTimer = 0;
    public bool canMove = true;//�Ƿ����ƶ�
    public bool isMoveReverse = false;
    public bool _isAniLeft = false;//��������
    [Header("����")]
    public int maxArmor;//��ػ�����
    public event Action OnShieldDamage;
    [HideInInspector]
    public int currentArmor;//��ػ�����
    public int restoreAmount;//���λ����޸���
    public float restoreCD;//�޸���ȴ
    private float _restoreTimer;
    private float lastHurtTimer;
    public float timeToRepairArmor;//��ս����޸�����


    [Header("����")]
    public WeaponDataSO mainWeapon;//������
    public WeaponDataSO secondaryWeapon;//������
    private int _currentMainAmmunition, _currentSecondaryAmmunition;//����������ǰ�ӵ���
    public float _mainAttackTimer, _secondaryAttackTimer;
    public bool canMainAttack, canSecondaryAttack;
    [HideInInspector]
    public Vector3 myAimPos;//��׼����

    [Header("ͨ��")]
    public bool hasDead;
    public OptionalFeature feature;//ѡ��ļ���
    [HideInInspector]
    public float _featureCDTimer;
    [HideInInspector]
    public float featureCD;
    [HideInInspector]
    public bool canUseFeature;
    public float waterMoveSoundPlayInterval;
    public float waterDashSoundPlayInterval;
    private float waterMoveSoundPlayTimer;
    private float waterDashSoundPlayTimer;
    public float hurtSoundPlayInterval;
    [HideInInspector]
    public float hurtSoundPlayTimer;

    public bool beCatched = false;


    [Header("����")]
    public float DistanceThreshold = 10;//������󳤶�
    public bool _hasConnected;//�Ƿ�������״̬
    public ObiRope _obiRope;
    public float switchStateBufferTime;//�Ͽ�������֮���л��Ļ���ʱ�� ��������������޷��Ͽ�������
    protected float switchStateBufferTimer;

    [Header("������Դ")]
    //public bool isDigging;
    //protected Resource _curDigRes;

    [Header("����")]
    public Item item;
    [HideInInspector]
    public Vector3 mouseWorldPS => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

    //�������
    [HideInInspector]
    public BubbleManager bubbleManager;


    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void PlayerInit()
    {
        _rigidbody = GetComponent<Rigidbody>();
        playerInput = transform.parent.GetComponent<PlayerInput>();
        playerInputSetting = transform.parent.GetComponent<PlayerInputSetting>();
        Debug.Log("Clone: "+transform.name+" / Index: "+myIndex);
        PlayerManager.Instance.gamePlayers.Add(gameObject);
        bubbleManager = GetComponent<BubbleManager>();
        //if (gameObject.CompareTag("Battery")) EnemyManager.Instance.battery = gameObject;
        myIndex = playerInput.playerIndex;
        currentArmor = maxArmor;
        lastHurtTimer = timeToRepairArmor;
        switchStateBufferTimer = switchStateBufferTime;
        _currentMainAmmunition = mainWeapon.initAmmunition;
        _currentSecondaryAmmunition = secondaryWeapon.initAmmunition;
        hasDead = false;
        //isDigging = false;
        _speedFactor = 1;
        _outSpeedFactor = 1;
        
        //�������
        animator = playerSprite.GetComponent<Animator>();
        _spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        
        ////�����ѡ����
        //feature = playerInputSetting.feature;
        Debug.Log(playerInputSetting.feature);
        
        //bubblePanel = GameObject.Find("BubblePanel").GetComponents<UIBubblePanel>()[0];
        
        //ѡ��ؿ�ʱʹ��UI��λӳ�䣬��֮����Playerӳ��
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
        if (!canMove)
        {
            _vertigoTimer += Time.deltaTime;
            if (_vertigoTimer >= vertigoTime)
            {
                canMove = true;
                _vertigoTimer = 0;
                _rigidbody.velocity = new(0, 0, 0);
            }
            return;
        }
        //if (isDigging) return;
        MovementCombination();
        if (!isMoveReverse)
        {
            transform.Translate(_moveDir * Time.deltaTime * speed * _speedFactor * _outSpeedFactor, Space.World);
        }
        else
        {
            transform.Translate(-_moveDir * Time.deltaTime * speed * _speedFactor * _outSpeedFactor, Space.World);
        }
        if (_moveDir != Vector3.zero && waterMoveSoundPlayTimer < 0)
        {
            MusicManager.Instance.PlaySound("ˮ�������ƶ�");
            waterMoveSoundPlayTimer = waterMoveSoundPlayInterval;
        }
        else if(waterMoveSoundPlayTimer >=0)
        {
            waterMoveSoundPlayTimer -= Time.deltaTime;
        }
        if(waterDashSoundPlayTimer>=0)
        {
            waterDashSoundPlayTimer -= Time.deltaTime;
        }
        
        //���������ж�
        if (playerInputSetting.GetAccelerateButtonDown())
        {
            _speedFactor = accelerateFactor;
            if(waterDashSoundPlayTimer<0)
            {
                MusicManager.Instance.PlaySound("ˮ�¿����ƶ�");
                waterDashSoundPlayTimer = waterDashSoundPlayInterval;
            }

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
    
    public void MoveAnimationControl(CharacterAnimation run_left,CharacterAnimation run_right,CharacterAnimation idle_left,CharacterAnimation idle_right)
    {
        if (playerInputSetting.inputDir.x != 0)
        {
            if (playerInputSetting.inputDir.x < 0)
            {
                _isAniLeft = true;
                animator.CrossFade(run_left.ToString(), 0f);
                _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetNormalByAnimationName(run_left));
                _spriteRenderer.material.SetTexture("_Emission",PlayerManager.Instance.GetEmissionByAnimationName(run_left));
            }
            else
            {
                _isAniLeft = false;
                animator.CrossFade(run_right.ToString(), 0f);
                _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetNormalByAnimationName(run_right));
                _spriteRenderer.material.SetTexture("_Emission",PlayerManager.Instance.GetEmissionByAnimationName(run_right));
            }
            
        }
        else
        {
            if (_isAniLeft)
            {
                animator.CrossFade(idle_left.ToString(),0);
                _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetNormalByAnimationName(idle_left));
                _spriteRenderer.material.SetTexture("_Emission",PlayerManager.Instance.GetEmissionByAnimationName(idle_left));
            }
            else
            {
                animator.CrossFade(idle_right.ToString(),0);
                _spriteRenderer.material.SetTexture("_Normal", PlayerManager.Instance.GetNormalByAnimationName(idle_right));
                _spriteRenderer.material.SetTexture("_Emission",PlayerManager.Instance.GetEmissionByAnimationName(idle_right));
            }
            
        }
    }
    
    
    /// <summary>
    /// ������������
    /// </summary>
    public void ReconnectRope()
    {
        _hasConnected = true;
        GetComponent<CellBattery>().ChangeConnectState(_hasConnected);
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

        MusicManager.Instance.PlaySound("���ӵ���");
        
        bubbleManager.DestroyBubble();
    }

    public void DisconnectRope()
    {
        if (_obiRope == null) return;
        MusicManager.Instance.PlaySound("�Ͽ�����");
        _hasConnected = false;
        GetComponent<CellBattery>().ChangeConnectState(_hasConnected);
        ObiParticleAttachment[] attachment = _obiRope.GetComponents<ObiParticleAttachment>();
        attachment[0].enabled = false;
        attachment[1].enabled = false;
        Destroy(_obiRope.gameObject, .5f);

    }

    public void Reborn()
    {
        hasDead = false;
        ReconnectRope();
    }

    /// <summary>
    /// �ж����ӳ����Ƿ񳬳���ֵ
    /// </summary>
    public void CheckDistanceToBattery()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SelectScene") return;
        if (!SceneManager.Instance.BatteryTransform) return;
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
            MusicManager.Instance.PlaySound("�Ͽ�����");
            Destroy(_obiRope.gameObject, .5f);

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
    public virtual void TakeDamage(int damage)
    {
        if (hasDead) return;
        Debug.Log(name);

        if(hurtSoundPlayTimer<0)
        {
            MusicManager.Instance.PlaySound("�������");
            hurtSoundPlayTimer = hurtSoundPlayInterval;
        }
        lastHurtTimer = 0;
        if (damage < currentArmor)
        {
            currentArmor -= damage;
            Instantiate(Resources.Load<GameObject>("Effect/Shield"),transform.position,Quaternion.identity,transform);
            OnShieldDamage?.Invoke();
        }
        else
        {
            if (playerInput.playerIndex!=0)
            {
                MusicManager.Instance.PlaySound("���ܻ�������");
            }

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
    /// ���������޸�����
    /// </summary>
    public void RestroeDefence()
    {
        //������뵽��ʴ״̬���ǾͲ��ؼ���
        if (isDefenceDowning)
        {
            if (defenceDownTimer < defenceDownTime)
            {
                defenceDownTimer += Time.deltaTime;
                //����
                currentArmor -= (int)(downRate * Time.deltaTime*50);
                currentArmor = Math.Max(currentArmor, 0);
                Debug.Log("��ʴ״̬����ǰ����ֵ��" + currentArmor);
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
                    SceneManager.Instance.BatteryTransform.GetComponent<MainBattery>().ChangePower(-restoreAmount);//�����������Ӧ�����޸�
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
    /// �������ף�����0���ߵ���ʱ����
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

    public void UpdateSwitchRopeState()
    {
        if(switchStateBufferTimer>=0)
        {
            switchStateBufferTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// ʹ�ü���
    /// </summary>
    //public virtual bool Skill()
    //{
    //    if(canUseSkill)
    //    {
    //        Debug.Log("ʹ�ü�����");
    //        _skillTimer = skillCD;
    //        canUseSkill = false;
    //        return true;
    //    }
    //    return false;
    //}
    /// <summary>
    /// ��ѣ��
    /// </summary>
    public virtual void Vertigo(Vector3 force,ForceMode forceMode=ForceMode.Impulse,float vertigoTime = 0.3f)
    {
        this.vertigoTime = vertigoTime;
        canMove = false;
        _rigidbody.AddForce(force, forceMode);
    }

    /// <summary>
    /// ��������״̬
    /// </summary>
    /// <param name="newState"></param>
    public void SetDeadState(bool newState)
    {
        hasDead = newState;
        if(hasDead)
        {
            MusicManager.Instance.PlaySound("��ɫ�����ľ�");
        }
    }

    /// <summary>
    /// ��������CD
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

    public void UpdateHurtSoundState()
    {
        if(hurtSoundPlayTimer>=0)
        {
            hurtSoundPlayTimer -= Time.deltaTime;
        }
    }


    /// <summary>
    /// ʹweapon��ǰ��ʼ�ճ���_aimDirection
    /// </summary>
    /// <param name="weapon">����</param>
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
    /// �������������� ��������д
    /// </summary>
    public virtual bool MainAttack()
    {
        //if (isDigging) return false;
        if (!canMainAttack||beCatched) return false;
        canMainAttack = false;
        _mainAttackTimer = mainWeapon.attackCD;
        return true;
    }

    /// <summary>
    /// �������������� ��������д
    /// </summary>
    public virtual bool SecondaryAttack() 
    {
        //if (isDigging) return false;
        if (!canSecondaryAttack||beCatched) return false;
        canSecondaryAttack = false;
        _secondaryAttackTimer = secondaryWeapon.attackCD;
        return true;
    }

    /// <summary>
    /// ���������崴��һ������UI���ǵ����˳�ʱɾ��
    /// </summary>
    /// <param name="other"></param>
    public void CreatBubbleUI(GameObject other)
    {
        switch (other.tag)
        {
            case "Resource":
                if (!hasDead)
                {
                    BubbleInfo info = new BubbleInfo
                    {
                        Type = BubbleType.Press,
                        Obj1 = gameObject,
                        Obj2 = other.gameObject,
                        Content = "�ɼ�"
                    };
                    bubbleManager.CreateBubble(info);
                }
                break;
            case "ReconnectArea":
                if (!_hasConnected&&!hasDead){
                BubbleInfo info = new BubbleInfo
                {
                    Type = BubbleType.Hold,
                    Obj1 = gameObject,
                    Obj2= other.gameObject,
                    Content = "��������"
                };
                bubbleManager.CreateBubble(info);
                }
                break;
            case "Chest":
                if (!hasDead)
                {
                    BubbleInfo info = new BubbleInfo
                    {
                        Type = BubbleType.Press,
                        Obj1 = gameObject,
                        Obj2 = other.gameObject,
                        Content = "��"
                    };
                    bubbleManager.CreateBubble(info);
                }


                break;
            default:
                break;
        }
    }


    public void ChangeSpeed(float ratio)
    {
        speed *= ratio;
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
