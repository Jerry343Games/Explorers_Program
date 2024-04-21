using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healer : PlayerController
{
    private bool isLeft = false;
    public GameObject gun;
    public Transform shootTransform;



    [Header("医者")]
    private int mainWeaponChargedAmount;
    public int maxChargedAmount;
    public Text chargedAmountText;


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
        UpdateSwitchRopeState();
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
        if (playerInputSetting.GetCableButtonDown() && _hasConnected && switchStateBufferTimer<0)
        {
            switchStateBufferTimer = switchStateBufferTime;
            DisconnectRope();
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
                other.GetComponent<Item>().Apply(gameObject);
                break;
            case "ResToCollecting":
                other.GetComponent<ResToCollecting>().Collecting();
                break;
            default:
                break;
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
                        //UIBubblePanel.Instance.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                    }
                }
                break;
            case "Resource":
                if (playerInputSetting.GetInteractButtonDown())
                {
                    other.GetComponent<Resource>().SpawnMineralCollections();
                }
                break;
            case "Chest":
                if (playerInputSetting.GetInteractButtonDown())
                {
                    other.GetComponent<PropChest>().OpenChest();
                }

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


                //UIBubblePanel.Instance.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "ReconnectArea":
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
            canMainAttack = true;
            _mainAttackTimer = mainWeapon.attackCD;
            mainWeaponChargedAmount++;
            mainWeaponChargedAmount = Mathf.Clamp(mainWeaponChargedAmount, 0, maxChargedAmount);
            chargedAmountText.text = mainWeaponChargedAmount.ToString();
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

        //if (!base.MainAttack())
        //{
        //    return false;
        //}
        MusicManager.Instance.PlaySound("医者");

        if (mainWeaponChargedAmount>0)
        {
            mainWeaponChargedAmount--;
            chargedAmountText.text = mainWeaponChargedAmount.ToString();

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
        if (!canUseFeature) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        MusicManager.Instance.PlaySound("麻醉枪");

        GetComponent<CellBattery>().ChangePower(-tranquilizerPower);
        //
        GameObject bullet = Instantiate(Resources.Load<GameObject>("TranquilizerBullet"), shootTransform.position, Quaternion.identity);
        bullet.GetComponent<TranquilizerBullet>().Init(tranquilizerWeaponData, gun.transform.forward, tranquilizerEffectTime);
    }


    //������̨
    public void FloatingFort()
    {
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
