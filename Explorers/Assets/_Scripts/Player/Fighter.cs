using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fighter : PlayerController
{
    
    public float attackAngle = 90f;
    public LayerMask enemyLayer;
    private List<GameObject> _enemyInArea=new();
    private bool isLeft = false;
    public float force = 5f;
    
    public Transform shootTransform;
    public int tempArmor;//��ʱ����
    public bool canFusionBomb = true;
    private Rigidbody _rb;
    public bool hasUseBomb = false;
    private bool _isAttack;
    private AniEventControl _myAniEventControl;

    //��ѡ����

    [Header("������")]

    public SonicWaveAttack sonicWaveAttack;
    public float startRadius = 0.4f;
    public float targetRadius = 100f;
    public float enemyVertigoTime = 2f;
    public float transitionDuration = 3f;
    public float sonicWaveCD;
    //private bool canSonicWave;

    [Header("��ײ")]

    private bool isDashing = false;
    public float dashTime = 3f;
    private float _dashTimer = 0;
    public float dashForce = 1f;
    public int dashDamage = 15;
    //private bool canDash;
    public float dashCD;
    public GameObject attcakAreaSprite;
    public GameObject attackAreaCollider;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        PlayerInit();
        _isAttack = false;
        _myAniEventControl = playerSprite.GetComponent<AniEventControl>();
        _myAniEventControl.OnFighterAttackEvent += OnHack;
        _myAniEventControl.EndFighterAttackEvent += EndHack;

    }

    private void OnDestroy()
    {
        _myAniEventControl.OnFighterAttackEvent -= OnHack;
        _myAniEventControl.OnFighterAttackEvent -= EndHack;
    }

    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
        UpdateFeatureState();
        UpdateSwitchRopeState();

        if (playerInputSetting.inputDir.x != 0)
        {
            if (playerInputSetting.inputDir.x < 0)
            {
                attackAreaCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
                attcakAreaSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                attackAreaCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
                attcakAreaSprite.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
        }
        if (playerInputSetting.GetAttackButtonDown())
        {           
            MainAttack();
        }
        else if (playerInputSetting.GetAttackSecondaryDown())
        {
            SecondaryAttack();
        }
        if(!isDashing) CharacterMove();

        RestroeDefence();
        
        //��ֹ�������ţ����������������ƶ�����
        if (!_isAttack)
        {
            MoveAnimationControlTest(CharacterAnimation.FighterLeft_Run, CharacterAnimation.FighterRight_Run, CharacterAnimation.FighterLeft_Walk, CharacterAnimation.FighterRight_Walk);

        }
        //UseItem();
        CheckDistanceToBattery();
        //CheckKeys();
        //������
        if (isDashing)
        {
            _rb.angularVelocity = Vector3.zero;
            if (_dashTimer < dashTime&&_rb.velocity.magnitude>0.1f)
            {
                _dashTimer += Time.deltaTime;
            }
            else
            {
                _dashTimer = 0;
                isDashing = false;
                _rb.mass = 1f;
                _rb.velocity = Vector3.zero;
                
            }
        }
        
        if (playerInputSetting.GetOptionalFeatureDown())
        {
            switch(feature)
            {
                case OptionalFeature.Dash:
                    Dash();
                    break;
                case OptionalFeature.SonicWave:
                    SonicWaveAttack();
                    break;
                default:
                    break;

            }
        }

        if(playerInputSetting.GetCableButtonDown() && _hasConnected &&switchStateBufferTimer<0)
        {
            switchStateBufferTimer = switchStateBufferTime;
            DisconnectRope();
        }

    }
    

    private void OnTriggerStay(Collider other)
    {
        if (hasDead) return;
        switch (other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if (!_hasConnected)
                {
                    if (playerInputSetting.GetCableButtonDown() && switchStateBufferTimer < 0)
                    {
                        switchStateBufferTimer = switchStateBufferTime;
                        ReconnectRope();
                        
                        //����������������ʾ����
                        //UIBubblePanel.Instance.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                    }
                }
                break;
            case "Resource":
                if(playerInputSetting.GetInteractButtonDown())
                {
                    other.GetComponent<Resource>().SpawnMineralCollections();
                }
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasDead && other.gameObject.tag == "ReconnectArea")
        {
            SceneManager.Instance.BatteryTransform.GetComponent<BatteryCarrier>().readyToRebornPlayers.Add(this);
            return;
        }

        CreatBubbleUI(other.gameObject);
        if (other.gameObject.CompareTag("Enemy"))
        {
            _enemyInArea.Add(other.gameObject);
        }
        if (other.gameObject.tag == "Item")
        {
            other.GetComponent<Item>().Apply(gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (hasDead && other.gameObject.tag == "ReconnectArea")
        {
            SceneManager.Instance.BatteryTransform.GetComponent<BatteryCarrier>().readyToRebornPlayers.Remove(this);
            return;
        }

        switch (other.tag)
        {
            case "Resource":


                //�뿪��Դ��������ٽ�������
                //UIBubblePanel.Instance.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "Enemy":
                if(_enemyInArea.Contains(other.gameObject))
                {
                    _enemyInArea.Remove(other.gameObject);
                }
                break;
            case "ReconnectArea":
                //�뿪���������������������ݾ�������
                if (UIBubblePanel.Instance.reconnectCableBuffer)
                {
                    UIBubblePanel.Instance.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();    
                }
                break;
            default:
                break;
        }

    }
    public override bool MainAttack()
    {
        if(!base.MainAttack())
        {
            return false;
        }
        
        _isAttack = true;
        attcakAreaSprite.SetActive(true);
        Invoke(nameof(AttackEffectDisable), Enemy.GetAnimatorLength(attcakAreaSprite.GetComponentInChildren<Animator>(), "FighterAttack"));
        //��������
        animator.CrossFade("FighterAttack",0);
        playerSprite.GetComponent<SpriteRenderer>().material.SetTexture("_Normal",
            PlayerManager.Instance.GetTextureByAnimationName(CharacterAnimation.FighterLeft_Attack));//???���ܴ�������
        if (_isAniLeft)
        {
            animator.CrossFade("FighterLeft_Attack", 0f);
            playerSprite.GetComponent<SpriteRenderer>().material.SetTexture("_Normal",
            PlayerManager.Instance.GetTextureByAnimationName(CharacterAnimation.FighterLeft_Attack));//???���ܴ�������
        }
        else
        {
            animator.CrossFade("FighterRight_Attack", 0f);
            playerSprite.GetComponent<SpriteRenderer>().material.SetTexture("_Normal",
            PlayerManager.Instance.GetTextureByAnimationName(CharacterAnimation.FighterRight_Attack));//???���ܴ�������
        }




        MusicManager.Instance.PlaySound("���⹥��");

        return true;
    }
    public void AttackEffectDisable()
    {
        attcakAreaSprite.SetActive(false);
        _isAttack = false;
    }
    /// <summary>
    /// ��֡�����¼�����ʵ�ʵ��˺�����
    /// </summary>
    private void OnHack()
    {
        Debug.Log("hit" + _enemyInArea.Count);
        if (_enemyInArea.Count == 0) return;
        

        for (int i = 0; i < _enemyInArea.Count; i++)
        {
            _enemyInArea[i].GetComponent<Enemy>().TakeDamage((int)mainWeapon.attackDamage);
            _enemyInArea[i].GetComponent<Enemy>().Vertigo(attackAreaCollider.transform.right.normalized*force);
            if (_enemyInArea[i].GetComponent<Enemy>().HP <= 0) _enemyInArea.RemoveAt(i);
            /*
            if (isLeft)
            {
                _enemyInArea[i].GetComponent<Enemy>().Vertigo(-transform.right * force);
            }
            else
            {
                _enemyInArea[i].GetComponent<Enemy>().Vertigo(transform.right * force);
            }*/
            
        }
    }

    private void EndHack()
    {
        _isAttack = false;
    }
    
    public override bool SecondaryAttack()
    {
        if(!base.SecondaryAttack())
        {
            return false;
        }
        if (hasUseBomb) return false; 
        hasUseBomb = true;
        GameObject bomb = Instantiate(Resources.Load<GameObject>("Bomb"), transform.position, Quaternion.identity);
        bomb.GetComponent<Bomb>().Init(secondaryWeapon, attackAreaCollider.transform.right,1,this);
        return true;
    }

    //��Ϊ���ؿ��Ի����ʱ���� ������дһ�����˷���
    public override void TakeDamage(int damage)
    {
        if (hasDead||isDashing) return;
        int realDamage = damage;
        if(damage<=tempArmor)
        {
            tempArmor-=damage;

        }
        else
        {
            realDamage = damage - tempArmor;
            tempArmor = 0;
        }

        if (realDamage < currentArmor)
        {
            currentArmor -= realDamage;
        }
        else
        {
            int damageToBattery = realDamage - currentArmor;
            GetComponent<Battery>().ChangePower(-damageToBattery);
            currentArmor = maxArmor;
        }
    }

    //��ײ
    public void Dash()
    {
        if (!canUseFeature) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;

        Vector3 moveDir = new Vector3(playerInputSetting.inputDir.x, playerInputSetting.inputDir.y, 0).normalized;
        if (moveDir.Equals(Vector3.zero)) return;
        isDashing = true;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        //_rb.isKinematic = true;
        _rb.mass = 5;
        _rb .AddForce(moveDir * dashForce, ForceMode.VelocityChange);
        
    }

    //������
    public void SonicWaveAttack()
    {
        if (!canUseFeature) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        sonicWaveAttack.AttactStart(this,startRadius, targetRadius, enemyVertigoTime,transitionDuration);
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Enemy")){
            collision.gameObject.GetComponent<Enemy>().TakeDamage(dashDamage);
        }
    }

    public override void Vertigo(Vector3 force, ForceMode forceMode = ForceMode.Impulse, float vertigoTime = 0.3F)
    {
        _rb.AddForce(force/2, forceMode);
    }

    public override void SetFeatureCD()
    {
        featureCD = feature switch
        {
            OptionalFeature.SonicWave => sonicWaveCD,
            OptionalFeature.Dash => dashCD,
            _ => 0,
        };
    }

}
