using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : PlayerController
{
    private bool isLeft = false;
    public GameObject gun;
    public Transform shootTransform;

    [Header("医者额外参数")]
    private int mainWeaponChargedAmount;//"医者"充能次数
    public int maxChargedAmount;//最大充能次数

    [Header("麻醉枪")]
    public WeaponDataSO tranquilizerWeaponData;//麻醉枪参数
    private int _currentTranquilizerAmmunition;//麻醉枪当前子弹数
    public int tranquilizerPower;//每发麻醉子弹耗电量
    private bool canTranquilizerAttack;
    private float _tranquilizerAttackTimer;
    public float tranquilizerEffectTime;//麻醉效果时间

    [Header("浮游炮台")]
    public WeaponDataSO fortWeaponData;//浮游炮台参数
    public float fortExitTime;//浮游炮台存在时间
    private float _fortExitTimer;
    public int fortPower;
    public float fortCD;//浮游炮台冷却时间
    private float _fortCDTimer;
    private bool hasExited;
    private bool canCallFort;
    public Transform fortPoint;
    private GameObject _curFort;
    void Awake()
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
        if (playerInputSetting.inputDir.x < 0)
        {
            transform.localScale = new(-1, 1, 1);
            isLeft = true;
        }
        else
        {
            isLeft = false;
            transform.localScale = new(1, 1, 1);
        }
        UseItem();
        CheckDistanceToBattery();
        CheckKeys();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        CreatBubbleUI(other.gameObject);
    }
    
    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            //进入可重连绳子区域
            case "ReconnectArea":
                if (!_hasConnected && playerInputSetting.GetCableButtonDown())
                {
                    ReconnectRope();
                    //重连后销毁重连提示气泡
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                }
                break;
            //收集到场景物品
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
                //离开资源区域后销毁交互气泡
                bubblePanel.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "ReconnectArea":
                //离开重连区域后如果有重连气泡就销毁下
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
    //“医者”
    public override bool MainAttack()
    {
        //充能型武器不用主武器那一套判断
        if (isDigging) return false;
        if (mainWeaponChargedAmount>0)
        {
            mainWeaponChargedAmount--;
            foreach (var player in EnemyManager.Instance.players)
            {
                if (player.name == "BatteryCarrier") continue;
                //非电池角色回复护盾
                PlayerController controller = player.GetComponent<PlayerController>();
                controller.currentArmor = Mathf.Min(controller.currentArmor + (int)mainWeapon.attackDamage, controller.maxArmor);
            }
        }
        return true;
    }

    //编织
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

    #region 自选功能
    //麻醉枪
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


    //浮游炮台
    public void FloatingFort()
    {
        if (isDigging) return;
        if (!canCallFort) return;
        hasExited = true;
        _fortExitTimer = fortExitTime;
        canCallFort = false;
        _fortCDTimer = fortCD;

        //创建浮游炮台
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
                //销毁
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
