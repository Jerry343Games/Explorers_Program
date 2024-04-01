using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : PlayerController
{
    private bool isLeft = false;
    public GameObject gun;
    public Transform shootTransform;

    [Header("ҽ�߶������")]
    private int mainWeaponChargedAmount;//"ҽ��"���ܴ���
    public int maxChargedAmount;//�����ܴ���

    [Header("����ǹ")]
    public WeaponDataSO tranquilizerWeaponData;//����ǹ����
    private int _currentTranquilizerAmmunition;//����ǹ��ǰ�ӵ���
    public int tranquilizerPower;//ÿ�������ӵ��ĵ���
    private bool canTranquilizerAttack;
    private float _tranquilizerAttackTimer;
    public float tranquilizerEffectTime;//����Ч��ʱ��

    [Header("������̨")]
    public WeaponDataSO fortWeaponData;//������̨����
    public float fortExitTime;//������̨����ʱ��
    private float _fortExitTimer;
    public int fortPower;
    public float fortCD;//������̨��ȴʱ��
    private float _fortCDTimer;
    private bool hasExited;
    private bool canCallFort;
    public Transform fortPoint;
    private GameObject _curFort;
    void Start()
    {
        PlayerInit();
        _currentTranquilizerAmmunition = tranquilizerWeaponData.initAmmunition;
        canCallFort = true;
    }
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
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
        AnimationControl(CharacterAnimation.HealerRun,CharacterAnimation.HealerIdle);
        
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
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                }
                break;
            //�ռ���������Ʒ
            case "Item":
                other.GetComponent<Item>().Apply(gameObject);
                break;
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
                bubblePanel.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "ReconnectArea":
                //�뿪���������������������ݾ�������
                if (bubblePanel.reconnectCableBuffer)
                {
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();    
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
            foreach (var player in PlayerManager.Instance.players)
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
        if(!base.SecondaryAttack())
        {
            return false;
        }
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(secondaryWeapons, new Vector3(transform.localScale.x, 0, 0));
        return true;
    }

    #region ��ѡ����
    //����ǹ
    public void TranquilizerGun()
    {
        if (isDigging) return;
        if(canTranquilizerAttack)
        {
            canTranquilizerAttack = false;
            _tranquilizerAttackTimer = tranquilizerWeaponData.attackCD;
            GetComponent<CellBattery>().ChangePower(-tranquilizerPower);
            //
            GameObject bullet = Instantiate(Resources.Load<GameObject>("TranquilizerBullet"), transform.position, Quaternion.identity);
            bullet.GetComponent<TranquilizerBullet>().Init(mainWeapon, gun.transform.forward,tranquilizerEffectTime);

        }
    }


    //������̨
    public void FloatingFort()
    {
        if (isDigging) return;
        if (!canCallFort) return;
        hasExited = true;
        _fortExitTimer = fortExitTime;
        canCallFort = false;
        _fortCDTimer = fortCD;

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
        if(!canCallFort)
        {
            _fortCDTimer -= Time.deltaTime;
            if(_fortCDTimer<0)
            {
                canCallFort = true;
            }
        }
        if (_tranquilizerAttackTimer < 0)
        {
            canTranquilizerAttack = true;
        }
        else
        {
            _tranquilizerAttackTimer -= Time.deltaTime;
        }
    }


}
