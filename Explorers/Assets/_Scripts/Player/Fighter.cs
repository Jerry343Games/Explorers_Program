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
    public int tempArmor;//临时护盾
    public bool canFusionBomb = true;
    private Rigidbody _rb;
    public bool hasUseBomb = false;
    private bool _isAttack;
    private AniEventControl _myAniEventControl;
    public float attackVertigoTime = 0.3f;
    //自选功能

    [Header("次声波")]

    public SonicWaveAttack sonicWaveAttack;
    public float startRadius = 0.4f;
    public float targetRadius = 100f;
    public float enemyVertigoTime = 2f;
    public float transitionDuration = 3f;
    public float sonicWaveCD;
    //private bool canSonicWave;

    [Header("冲撞")]

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
        
        //防止动画干扰，攻击结束后再做移动动画
        if (!_isAttack)
        {
            MoveAnimationControl(CharacterAnimation.FighterLeft_Run, CharacterAnimation.FighterRight_Run, CharacterAnimation.FighterLeft_Walk, CharacterAnimation.FighterRight_Walk);

        }
        //UseItem();
        CheckDistanceToBattery();
        //CheckKeys();
        //冲刺相关
        if (isDashing)
        {
            _rb.angularVelocity = Vector3.zero;
            if (_dashTimer < dashTime&&_rb.velocity.magnitude>1f)
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
            //?????????????????
            case "ReconnectArea":
                if (!_hasConnected)
                {
                    if (playerInputSetting.GetCableButtonDown() && switchStateBufferTimer < 0)
                    {
                        switchStateBufferTimer = switchStateBufferTime;
                        ReconnectRope();
                        bubbleManager.DestroyBubble();

                    }
                }
                break;
            case "Resource":
                if (playerInputSetting.GetInteractButtonDown())
                {
                    other.GetComponent<Resource>().BeginingDigging();
                    bubbleManager.DestroyBubble();

                }
                break;
            case "Chest":
                if (playerInputSetting.GetInteractButtonDown())
                {
                    other.GetComponent<PropChest>().OpenChest();
                    bubbleManager.DestroyBubble();

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
        switch (other.gameObject.tag)
        {
            case "Item":
                MusicManager.Instance.PlaySound("收集");

                other.GetComponent<Item>().Apply(gameObject);
                break;
            case "ResToCollecting":
                MusicManager.Instance.PlaySound("收集");

                other.GetComponent<ResToCollecting>().Collecting();
                break;
            case "Enemy":
                _enemyInArea.Add(other.gameObject);
                break;
            default:
                break;
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
                bubbleManager.DestroyBubble();


                break;
            case "Enemy":
                if(_enemyInArea.Contains(other.gameObject))
                {
                    _enemyInArea.Remove(other.gameObject);
                }
                break;
            case "ReconnectArea":
                bubbleManager.DestroyBubble();


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
        //动画控制
        animator.CrossFade("FighterAttack",0);
        playerSprite.GetComponent<SpriteRenderer>().material.SetTexture("_Normal",
            PlayerManager.Instance.GetTextureByAnimationName(CharacterAnimation.FighterLeft_Attack));//???可能存在问题
        if (_isAniLeft)
        {
            animator.CrossFade("FighterLeft_Attack", 0f);
            playerSprite.GetComponent<SpriteRenderer>().material.SetTexture("_Normal",
            PlayerManager.Instance.GetTextureByAnimationName(CharacterAnimation.FighterLeft_Attack));//???可能存在问题
        }
        else
        {
            animator.CrossFade("FighterRight_Attack", 0f);
            playerSprite.GetComponent<SpriteRenderer>().material.SetTexture("_Normal",
            PlayerManager.Instance.GetTextureByAnimationName(CharacterAnimation.FighterRight_Attack));//???可能存在问题
        }




        MusicManager.Instance.PlaySound("链锯攻击");

        return true;
    }
    public void AttackEffectDisable()
    {
        attcakAreaSprite.SetActive(false);
        _isAttack = false;
    }
    /// <summary>
    /// 由帧动画事件触发实际的伤害结算
    /// </summary>
    private void OnHack()
    {

        // 创建一个新的列表，用于存储仍然存活的敌人
        List<GameObject> aliveEnemies = new List<GameObject>(_enemyInArea.Count);

        foreach (GameObject enemyObj in _enemyInArea)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy.HP > 0)
            {
                enemy.TakeDamage((int)mainWeapon.attackDamage);
                // 对敌人造成伤害后震慑它们
                enemy.Vertigo(attackAreaCollider.transform.right * force,ForceMode.VelocityChange,attackVertigoTime);
                aliveEnemies.Add(enemy.gameObject);
            }
        }

        // 更新列表，剔除已被消灭的敌人
        _enemyInArea = aliveEnemies;

        
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

    //因为超载可以获得临时护盾 所以重写一下受伤方法
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

    //冲撞
    public void Dash()
    {
        if (!canUseFeature) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        MusicManager.Instance.PlaySound("冲锋");
        Vector3 moveDir = new Vector3(playerInputSetting.inputDir.x, playerInputSetting.inputDir.y, 0).normalized;
        if (moveDir.Equals(Vector3.zero)) return;
        isDashing = true;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        //_rb.isKinematic = true;
        _rb.mass = 5;
        _rb .AddForce(moveDir * dashForce, ForceMode.VelocityChange);
        
    }

    //次声波
    public void SonicWaveAttack()
    {
        if (!canUseFeature) return;
        MusicManager.Instance.PlaySound("次声波");

        canUseFeature = false;
        _featureCDTimer = featureCD;
        Instantiate(Resources.Load<GameObject>("Effect/WaterDistortion"), transform.position,Quaternion.identity);
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
