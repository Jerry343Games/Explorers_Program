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
    private bool hasUseBomb = false;
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
        if (playerInputSetting.GetAttackButtonDown())
        {           
            MainAttack();
        }else if (playerInputSetting.GetAttackSecondaryDown())
        {
            SecondaryAttack();
        }
        if(!isDashing) CharacterMove();

        RestroeDefence();
        
        //��ֹ�������ţ����������������ƶ�����
        if (!_isAttack)
        {
            MoveAnimationControl(CharacterAnimation.FighterRun,CharacterAnimation.FighterWalk);
        }
        //UseItem();
        CheckDistanceToBattery();
        //CheckKeys();
        //������
        if (isDashing)
        {
            _rb.angularVelocity = Vector3.zero;
            if (_dashTimer < dashTime)
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
    }
    

    private void OnTriggerStay(Collider other)
    {
        switch(other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if (!_hasConnected)
                {
                    if (playerInputSetting.GetCableButtonDown())
                    {
                        ReconnectRope();
                        
                        //����������������ʾ����
                        bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                    }
                }
                break;
            //�ռ���������Ʒ
            case "Item":
                if((other.transform.position-transform.position).magnitude<0.3f)
                other.GetComponent<Item>().Apply(gameObject);
                break;
            case "Resource":
                if(!isDigging&& playerInputSetting.GetInteractButtonDown())
                {
                    isDigging = true;
                    _curDigRes = other.GetComponent<Resource>();
                }
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        CreatBubbleUI(other.gameObject);
        if (other.gameObject.CompareTag("Enemy"))
        {
            _enemyInArea.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {

        switch (other.tag)
        {
            case "Resource":
                if (isDigging)
                {
                    isDigging = false;
                   
                    _curDigRes = null;
                }
                //�뿪��Դ��������ٽ�������
                bubblePanel.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "Enemy":
                if(_enemyInArea.Contains(other.gameObject))
                {
                    _enemyInArea.Remove(other.gameObject);
                }
                break;
            case "ReconnectArea":
                //�뿪���������������������ݾ�������
                if (bubblePanel.reconnectCableBuffer)
                {
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();    
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
        Invoke(nameof(AttackEffectDisable), Enemy.GetAnimatorLength(attcakAreaSprite.GetComponent<Animator>(), "FighterAttack"));
        //��������
        animator.CrossFade("FighterAttack",0);
        playerSprite.GetComponent<SpriteRenderer>().material.SetTexture("_Normal",
            PlayerManager.Instance.GetTextureByAnimationName(CharacterAnimation.FighterAttack));
        return true;
    }
    public void AttackEffectDisable()
    {
        attcakAreaSprite.SetActive(false);
    }
    /// <summary>
    /// ��֡�����¼�����ʵ�ʵ��˺�����
    /// </summary>
    private void OnHack()
    {
        if (_enemyInArea.Count == 0) return;
        Debug.Log(_enemyInArea.Count);

        for (int i = 0; i < _enemyInArea.Count; i++)
        {
            _enemyInArea[i].GetComponent<Enemy>().TakeDamage((int)mainWeapon.attackDamage);
            _enemyInArea[i].GetComponent<Enemy>().Vertigo(transform.GetChild(0).transform.forward*force);
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
        bomb.GetComponent<Bomb>().Init(secondaryWeapons, transform.GetChild(0).transform.forward);
        return true;
    }

    //��Ϊ���ؿ��Ի����ʱ���� ������дһ�����˷���
    public override void TakeDamage(int damage)
    {
        if (hasDead||isDashing) return;
        if (isDigging)
        {
            isDigging = false;//���״̬
           
        }
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
        if (isDigging) return;
        if (!canUseFeature) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        Vector3 moveDir = new Vector3(playerInputSetting.inputDir.x, playerInputSetting.inputDir.y, 0).normalized;
        if (moveDir.Equals(Vector3.zero)) return;
        isDashing = true;
        //_rb.isKinematic = true;
        _rb.mass = 5;
        _rb .AddForce(moveDir * dashForce, ForceMode.VelocityChange);
        
    }

    //������
    public void SonicWaveAttack()
    {

        if (isDigging) return;
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
