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
    [HideInInspector]
    public UIBubblePanel bubblePanel;
    
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
    private bool _canMove = true;//�Ƿ����ƶ�
    public bool isMoveReverse = false;
    public bool _isAniLeft = false;//��������
    [Header("����")]
    public int maxArmor;//��ػ�����
    [HideInInspector]
    public int currentArmor;//��ػ�����
    public int restoreAmount;//���λ����޸���
    public float restoreCD;//�޸���ȴ
    private float _restoreTimer;
    private float lastHurtTimer;
    public float timeToRepairArmor;//��ս����޸�����


    [Header("����")]
    public WeaponDataSO mainWeapon;//������
    public WeaponDataSO secondaryWeapons;//������
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

    [Header("����")]
    public float DistanceThreshold = 10;//������󳤶�
    protected bool _hasConnected;//�Ƿ�������״̬
    protected ObiRope _obiRope;

    [Header("������Դ")]
    public bool isDigging;
    protected Resource _curDigRes;

    [Header("����")]
    public Item item;
    [HideInInspector]
    public Vector3 mouseWorldPS => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));




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
        
        //�������
        animator = playerSprite.GetComponent<Animator>();
        _spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        
        ////�����ѡ����
        //feature = playerInputSetting.feature;
        Debug.Log(playerInputSetting.feature);
        
        bubblePanel = GameObject.Find("BubblePanel").GetComponents<UIBubblePanel>()[0];
        
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
        
        
        //���������ж�
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
    /// ��������
    /// </summary>
    /// <param name="run">�ƶ�������</param>
    /// <param name="idle">����������</param>
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
            //���ɶ������л�����
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
        if(isDigging)
        {
            isDigging = false;//���״̬
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
            MusicManager.Instance.PlaySound("���ܻ�������");

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
                currentArmor -= (int)(downRate * Time.deltaTime);
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
        _canMove = false;
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
        if (isDigging) return false;
        if (!canMainAttack) return false;
        canMainAttack = false;
        _mainAttackTimer = mainWeapon.attackCD;
        return true;
    }

    /// <summary>
    /// �������������� ��������д
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
                    BubbleInfo resInfo = new BubbleInfo
                    {
                        Type = BubbleType.ResourceCollectionBubble,
                        Obj1 = gameObject,
                        Obj2 = other.gameObject,
                        Content = "�ɼ�"
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
                    Content = "��������"
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
                        Content = "��"
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
