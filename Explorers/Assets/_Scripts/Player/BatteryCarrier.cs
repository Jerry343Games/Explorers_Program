using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BatteryCarrier : PlayerController
{


    [Header("超载")]
    public PlayerController selectedPlayer;//选择的要进行操作的机器人
    public Image selectPlayerImage;

    public float overloadDuration;//过载持续时间
    public float overloadCD;//过载功能冷却时间
    public int overloadPower;//过载功能耗电量
    private bool isOverloading;
    private bool canOverload;
    private float _overloadTimer;
    private float _overloadCDTimer;
    private PlayerController _overloadPlayer;
    private string tempName;

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
    public int chargePower;//充能所需耗电量

    [Header("后勤")]
    public int logisticsPower;//后勤功能耗电量
    public float logisticsCD;//后勤功能CD

    [Header("救援")]
    public List<PlayerController> readyToRebornPlayers = new List<PlayerController>();
    void Start()
    {
        PlayerInit();
        canOverload = true;
        if(PlayerManager.Instance)
        {
            PlayerManager.Instance.hasMainBattary = true; //通知电池加入，给其他职业监听以获得主电池
        }
        if(EnemyManager.Instance)
        {
            EnemyManager.Instance.battery = this.gameObject;
        }
        myPlayerInfo = new PlayerInfo(PlayerType.BatteryCarrier, speed, maxArmor, mainWeapon, secondaryWeapon);
        PlayerManager.Instance?.AddPlayerInfo(myPlayerInfo);
    }
    void Update()
    {
        if (hasDead) return;
        if (SceneManager.Instance)
        {
            if(SceneManager.Instance.hasGameOver)
            {
                return;
            }
        }

        CharacterMove();
        //CheckKeys();
        TickTime();
        UpdateFeatureState();
        PressPause();
        if(playerInputSetting.GetAttackButtonDown())
        {
            Overload();
        }
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
        MoveAnimationControl(CharacterAnimation.BatteryLeft_Run, CharacterAnimation.BatteryRight_Run, CharacterAnimation.BatteryLeft_Walk, CharacterAnimation.BatteryRight_Walk);

        if (playerInputSetting.GetSelectCombination()==Vector3.right)
        {
            selectedPlayer = FindObjectsOfType<PlayerController>().ToList().Find(x => x.gameObject.name == "Shooter");

            SetSelectPlayerImage(PlayerType.Shooter);
        }
        else if(playerInputSetting.GetSelectCombination() == Vector3.up)
        {
            selectedPlayer = FindObjectsOfType<PlayerController>().ToList().Find(x => x.gameObject.name == "Fighter");
            SetSelectPlayerImage(PlayerType.Fighter);

        }
        else if(playerInputSetting.GetSelectCombination() == Vector3.forward)
        {
            selectedPlayer = FindObjectsOfType<PlayerController>().ToList().Find(x => x.gameObject.name == "Healer");
            SetSelectPlayerImage(PlayerType.Healer);

        }

        if(readyToRebornPlayers.Count>0 && playerInputSetting.GetCableButtonDown())
        {
            readyToRebornPlayers[0].Reborn();
            readyToRebornPlayers.RemoveAt(0);
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
        MusicManager.Instance.PlaySound("超载");

        GetComponent<MainBattery>().ChangePower(-overloadPower);
        Instantiate(Resources.Load<GameObject>("Effect/SparkYellow"), transform.position, Quaternion.identity);
        GameObject upEffect = Instantiate(Resources.Load<GameObject>("Effect/PowerUp"), selectedPlayer.transform.position, Quaternion.identity);
        upEffect.transform.SetParent(selectedPlayer.transform);
        Destroy(upEffect, overloadDuration);

        tempName = selectedPlayer.gameObject.name;
        switch (selectedPlayer.gameObject.name)
        {
            case "Shooter"://射手
                selectedPlayer.mainWeapon.attackDamage *= 1.5f;
                selectedPlayer.secondaryWeapon.attackDamage *= 1.5f;
                Debug.Log("强化了射手");
                break;
            case "Fighter"://战士
                (selectedPlayer as Fighter).tempArmor = selectedPlayer.maxArmor;
                Debug.Log("强化了战士");
                break;
            case "Healer"://医疗
                selectedPlayer.mainWeapon.attackDamage *= 2;
                Debug.Log("强化了医疗");

                break;
            default:
                break;
        }
    }
    public void BackFormOverload()
    {
        switch (tempName)
        {
            case "Shooter"://射手
                selectedPlayer.mainWeapon.attackDamage /= 1.5f;
                selectedPlayer.secondaryWeapon.attackDamage /= 1.5f;
                break;
            case "Fighter"://战士
                (selectedPlayer as Fighter).tempArmor = 0;
                break;
            case "Healer"://医疗
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

        MusicManager.Instance.PlaySound("电弧打击");
        canLightningAttack = false;
        _lightningAttackTimer = lightningAttackCD;
        Instantiate(Resources.Load<GameObject>("Effect/SparkBlue"), transform.position, Quaternion.identity);
        int attackCount = Mathf.Min(colliders.Length, targetNumber); // 实际攻击的目标数
        GetComponent<MainBattery>().ChangePower(-lightningAttackPower * attackCount); // 能量消耗根据实际攻击的目标数而定
        CameraTrace.instance.CameraZoom(0.5f,90f,0f,1f);
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
        if (!canUseFeature) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        GetComponent<MainBattery>().ChangePower(-chargePower);
        MusicManager.Instance.PlaySound("充能");
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player == this) continue;
            player.currentArmor = player.maxArmor;
        }

    }
    #endregion

    #region 后勤
    public void Logistics()
    {
        if (!canUseFeature || !selectedPlayer) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        GetComponent<MainBattery>().ChangePower(-logisticsPower);
        //MusicManager.Instance.PlaySound("超载");

        //没有就获得道具 有了就替换原来的道具
        int randomItem = Random.Range(0, 3);
        //最喜欢的swith 获得道具 这样行？
        switch(randomItem)
        {
            case 0:
                GravityWell well = Instantiate(Resources.Load<GameObject>("Item/GravityWell"), selectedPlayer.transform.position, Quaternion.identity).GetComponent<GravityWell>();
                break;
            case 1:
                PropelBackpack backpack = Instantiate(Resources.Load<GameObject>("Item/PropelBackpack"), selectedPlayer.transform.position, Quaternion.identity).GetComponent<PropelBackpack>();
                break;
            case 2:
                Sonar sonar = Instantiate(Resources.Load<GameObject>("Item/Sonar"), selectedPlayer.transform.position, Quaternion.identity).GetComponent<Sonar>();
                break;
            default:
                break;
        }

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

    }

    public override void SetFeatureCD()
    {
        featureCD = feature switch
        {
            OptionalFeature.Logistics => logisticsCD,
            OptionalFeature.Charging => chargeCD,
            _ => 0,
        };
    }

    public void SetSelectPlayerImage(PlayerType type)
    {
        selectPlayerImage.color = Color.white;
        selectPlayerImage.sprite = Resources.Load<Sprite>("UI/Image/"+type.ToString());
    }


    public override void AddBuff(UpgradeBuff buff)
    {
        base.AddBuff(buff);
        switch(buff.buffName)
        {
            case "扩容电池":
                GetComponent<MainBattery>().maxPower += 200;
                GetComponent<MainBattery>().currentPower += 200;
                break;
            case "号令":
                overloadDuration += 1.5f;
                overloadPower += 2;
                break;
            case "多线程":
                targetNumber += 1;
                break;
            case "混乱变压器":
                lightningAttackDamage += 1;
                lightningAttackRange -= 0.4f;
                lightningAttackPower -= 1;
                break;
            default:
                break;

        }
    }
}
