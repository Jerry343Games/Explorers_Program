using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BatteryCarrier : PlayerController
{


    [Header("超载")]
    public PlayerController selectedPlayer;//选择的要进行操作的机器人

    public float overloadDuration;//过载持续时间
    public float overloadCD;//过载功能冷却时间
    public int overloadPower;//过载功能耗电量
    private bool isOverloading;
    private bool canOverload;
    private float _overloadTimer;
    private float _overloadCDTimer;
    private PlayerController _overloadPlayer;

    [Header("电弧打击")]
    public float lightningAttackCD;//电弧打击冷却时间
    public int targetNumber;
    private float _lightningAttackTimer;
    public int lightningAttackDamage;//电弧打击伤害
    public float lightningAttackRange;//电弧打击范围
    public int lightningAttackPower;//电弧打击耗电量
    private bool canLightningAttack;
    public LayerMask enemyLayer;

    [Header("充能")]
    public float chargeCD;//充能冷却时间
    private float _chargeCDTimer;
    public int chargePower;//充能所需耗电量
    private bool canCharge;
    void Start()
    {
        PlayerInit();
        canOverload = true;
        SceneManager.Instance.hasMainBattary = true; //通知电池加入，给其他职业监听以获得主电池
        
    }
    void Update()
    {
        if (hasDead) return;
        CharacterMove();
        //CheckKeys();
        TickTime();

        if (playerInputSetting.GetAttackSecondaryDown())
        {
            Lightning();
        }
        
        if (playerInputSetting.GetOptionalFeatureDown())
        {
            switch (feature)
            {
                case OptionalFeature.Logistics:
                    Logistics();
                    break;
                case OptionalFeature.Charging:
                    Charge();
                    break;
                default:
                    break;

            }
        }
    }

    #region 超载
    public void Overload()
    {
        if (!canOverload || !selectedPlayer) return;
        isOverloading = true;
        canOverload = false;
        _overloadTimer = overloadDuration;
        _overloadCDTimer = overloadCD;
        _overloadPlayer = selectedPlayer;

        GetComponent<MainBattery>().ChangePower(-overloadPower);
        switch (selectedPlayer.myIndex)
        {
            case 1://射手
                selectedPlayer.mainWeapon.attackDamage *= 1.5f;
                selectedPlayer.secondaryWeapons.attackDamage *= 1.5f;
                break;
            case 2://战士
                (selectedPlayer as Fighter).tempArmor = selectedPlayer.maxArmor;
                break;
            case 3://医疗
                selectedPlayer.mainWeapon.attackDamage *= 2;
                break;
            default:
                break;
        }
    }
    public void BackFormOverload()
    {
        switch (selectedPlayer.myIndex)
        {
            case 1://射手
                selectedPlayer.mainWeapon.attackDamage /= 1.5f;
                selectedPlayer.secondaryWeapons.attackDamage /= 1.5f;
                break;
            case 2://战士
                (selectedPlayer as Fighter).tempArmor = 0;
                break;
            case 3://医疗
                selectedPlayer.mainWeapon.attackDamage /= 2;
                break;
            default:
                break;
        }
    }

    #endregion

    #region 电弧打击
    public void Lightning()
    {
        if (!canLightningAttack) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, lightningAttackRange, enemyLayer);
        if (colliders.Length == 0) return;
        canLightningAttack = false;
        _lightningAttackTimer = lightningAttackCD;
        Instantiate(Resources.Load<GameObject>("Effect/SparkBlue"), transform.position, Quaternion.identity);
        int attackCount = Mathf.Min(colliders.Length, targetNumber); // 实际攻击的目标数
        GetComponent<MainBattery>().ChangePower(-lightningAttackPower * attackCount); // 能量消耗根据实际攻击的目标数而定
        CameraTrace.instance.CameraZoom(0.5f,90f,0f,0.5f);
        for (int i = 0; i < attackCount; i++)
        {
            GameObject lightning = Instantiate(Resources.Load<GameObject>("Lightning"), 
                (colliders[i].transform.position + transform.position) / 2, Quaternion.identity);
            Lightning lightComponent= lightning.GetComponent<Lightning>();
            lightComponent.startPos=transform;
            lightComponent.endPos = colliders[i].transform;
            lightComponent.Init(lightningAttackDamage,colliders[i].gameObject);
        }
    }
    #endregion

    #region 充能
    public void Charge()
    {
        if (!canCharge) return;
        canCharge = false;
        _chargeCDTimer = chargeCD;
        GetComponent<MainBattery>().ChangePower(-chargePower);
        foreach(var player in FindObjectsOfType<PlayerController>())
        {
            if (player == this) continue;
            player.currentArmor = player.maxArmor;
        }

    }
    #endregion

    #region 后勤
    public void Logistics()
    {

    }
    #endregion

    public void TickTime()
    {
        if(isOverloading)
        {
            _overloadTimer -= Time.deltaTime;
            if(_overloadTimer<0)
            {
                isOverloading = false;
                BackFormOverload();
            }
        }
        if(!canOverload)
        {
            _overloadCDTimer -= Time.deltaTime;
            if(_overloadCDTimer<0)
            {
                canOverload = true;
            }
        }
        if(!canLightningAttack)
        {
            _lightningAttackTimer -= Time.deltaTime;
            if(_lightningAttackTimer<0)
            {
                canLightningAttack = true;
            }
        }
        if(!canCharge)
        {
            _chargeCDTimer -= Time.deltaTime;
            if(_chargeCDTimer<0)
            {
                canCharge = true;
            }
        }

    }
}
