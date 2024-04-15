using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : PlayerController
{
    private bool isLeft = false;
    public GameObject gun;
    public Transform shootTransform;

    [Header("医者")]
    private int mainWeaponChargedAmount;//"ҽ��"���ܴ���
    public int maxChargedAmount;//�����ܴ���

    [Header("麻醉枪")]
    public WeaponDataSO tranquilizerWeaponData;
    //private int _currentTranquilizerAmmunition;
    public int tranquilizerPower;
    //private bool canTranquilizerAttack;
    //private float _tranquilizerAttackTimer;
    public float tranquilizerEffectTime;

    [Header("浮游炮台")]
    public WeaponDataSO fortWeaponData;
    public float fortExitTime;
    private float _fortExitTimer;
    public int fortPower;
    public float fortCD;
    private float _fortCDTimer;
    private bool hasExited;
    //private bool canCallFort;
    public Transform fortPoint;
    private GameObject _curFort;
    void Start()
    {
        PlayerInit();
        //_currentTranquilizerAmmunition = tranquilizerWeaponData.initAmmunition;
        //canCallFort = true;

    }
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
        UpdateFeatureState();
        Aim(gun);
        TimeTick();
        if (playerInputSetting.GetAttackButtonDown())
        {
            MainAttack();
        }
        else if (playerInputSetting.GetAttackSecondaryDown())
        {
            SecondaryAttack();
        }
        CharacterMove();
        RestroeDefence();
        UseItem();
        CheckDistanceToBattery();
        MoveAnimationControlTest(CharacterAnimation.HealerLeft_Run, CharacterAnimation.HealerRight_Run, CharacterAnimation.HealerLeft_Idle, CharacterAnimation.HealerRight_Idle);
        WeaponLayerChange();

        //CheckKeys();
        if (playerInputSetting.GetOptionalFeatureDown())
        {
            switch (feature)
            {
                case OptionalFeature.TranquilizerGun:
                    TranquilizerGun();
                    break;
                case OptionalFeature.FloatingFort:
                    FloatingFort();
                    break;
                default:
                    break;

            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        CreatBubbleUI(other.gameObject);
        if (other.gameObject.tag == "Item")
        {
            other.GetComponent<Item>().Apply(gameObject);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if (!_hasConnected && playerInputSetting.GetCableButtonDown())
                {
                    ReconnectRope();
                    //����������������ʾ����
                    UIBubblePanel.Instance.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                }
                break;
            ////�ռ���������Ʒ
            //case "Item":
            //    other.GetComponent<Item>().Apply(gameObject);
            //    break;
            case "Resource":
                
                if (!isDigging && playerInputSetting.GetInteractButtonDown())
                {
                    isDigging = true;
                    _curDigRes = other.GetComponent<Resource>();
                    _curDigRes.SetDiager(this);
                    _curDigRes.beDigging = true;
                }
                break;
            default:
                break;
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
                    _curDigRes.beDigging = false;
                    _curDigRes.SetDiager(null);
                    _curDigRes = null;
                }
                //�뿪��Դ��������ٽ�������
                UIBubblePanel.Instance.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "ReconnectArea":
                //�뿪���������������������ݾ�������
                if (UIBubblePanel.Instance.reconnectCableBuffer)
                {
                    UIBubblePanel.Instance.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();    
                }
                break;
        }
        
    }

    public override void UpdateAttackState()
    {
        if (_mainAttackTimer < 0)
        {
            _mainAttackTimer = mainWeapon.attackCD;
            mainWeaponChargedAmount++;
            mainWeaponChargedAmount = Mathf.Clamp(mainWeaponChargedAmount, 0, maxChargedAmount);
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
    //��ҽ�ߡ�
    public override bool MainAttack()
    {
        //����������������������һ���ж�
        if (isDigging) return false;
        if (mainWeaponChargedAmount>0)
        {
            mainWeaponChargedAmount--;
            foreach (var player in PlayerManager.Instance.gamePlayers)
            {
                if (player.name == "BatteryCarrier") continue;
                //�ǵ�ؽ�ɫ�ظ�����
                PlayerController controller = player.GetComponent<PlayerController>();
                controller.currentArmor = Mathf.Min(controller.currentArmor + (int)mainWeapon.attackDamage, controller.maxArmor);
            }
        }
        return true;
    }

    //��֯
    public override bool SecondaryAttack()
    {
        if (!base.SecondaryAttack())
        {
            return false;
        }
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), shootTransform.position, Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Effect/FlashSpiky"), shootTransform.position,
            gun.transform.rotation, shootTransform);
        bullet.GetComponent<Bullet>().Init(secondaryWeapon, gun.transform.forward);
        MusicManager.Instance.PlaySound("手枪射击");
        return true;
    }

    #region ��ѡ����
    //����ǹ
    public void TranquilizerGun()
    {
        if (isDigging) return;
        if (!canUseFeature) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        GetComponent<CellBattery>().ChangePower(-tranquilizerPower);
        //
        GameObject bullet = Instantiate(Resources.Load<GameObject>("TranquilizerBullet"), shootTransform.position, Quaternion.identity);
        bullet.GetComponent<TranquilizerBullet>().Init(tranquilizerWeaponData, gun.transform.forward, tranquilizerEffectTime);
    }


    //������̨
    public void FloatingFort()
    {
        if (isDigging) return;
        if (!canUseFeature) return;
        hasExited = true;
        _fortExitTimer = fortExitTime;
        canUseFeature = false;
        _featureCDTimer = featureCD;

        //����������̨
        GameObject floatingFort = Instantiate(Resources.Load<GameObject>("FloatingFort"), transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
        floatingFort.transform.SetParent(transform);
        GetComponent<CellBattery>().ChangePower(-fortPower);
        floatingFort.GetComponent<FloatingFort>().Init(fortWeaponData, fortPoint);
        _curFort = floatingFort;

    }



    #endregion

    public void TimeTick()
    {
        if(hasExited)
        {
            _fortExitTimer-= Time.deltaTime;
            if(_fortExitTimer<0)
            {
                //����
                Destroy(_curFort);
                hasExited = false;
            }
        }
    }

    public override void SetFeatureCD()
    {
        featureCD = feature switch
        {
            OptionalFeature.TranquilizerGun => tranquilizerWeaponData.attackCD,
            OptionalFeature.FloatingFort => fortCD,
            _ => 0,
        };
    }
    public void WeaponLayerChange()
    {
        if (_isAniLeft)
        {
            gun.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
        else
        {
            gun.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = -1;
        }
    }

}
