using DG.Tweening;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

public class Shooter : PlayerController
{
    private bool isLeft;
    
    public Transform shootTransform;
    [Header("齐射")]
    public float salvoCD;//齐射冷却时间
    //private float _salvoCDTimer;
    public int salveAmount;//齐射的导弹数量
    public int salveMissileDamage;//单枚导弹伤害
    public float salvoRange;//齐射检测范围
    public float salvoMissileSpeed;//单枚导弹速度
    public int salvoPower;//齐射耗电量
    //private bool canSalvo;
    public LayerMask enemyLayer;

    [Header("毁灭鱼雷")]
    public float torpedoesCD;//毁灭鱼雷冷却时间
    //private float _torpedoesCDTimer;
    public float torpedoesRange;//毁灭鱼雷爆炸范围
    public float destoryTime;//毁灭鱼雷销毁时间
    public float torpedoesSpeed;//毁灭鱼雷飞行速度
    public float torpedoesForce;//毁灭鱼雷击退力
    public int torpedoesPower;//毁灭鱼雷消耗的电量
    public int torpedoesDamage;
    //private bool canTorpedoes;
    public LayerMask playerLayer;

    void Start()
    {
        PlayerInit();
        myPlayerInfo = new PlayerInfo(PlayerType.Shooter, speed, maxArmor, mainWeapon, secondaryWeapon);
        PlayerManager.Instance.AddPlayerInfo(myPlayerInfo);
    }
    // Update is called once per frame
    void Update()
    {
        if (hasDead) return;
        if (SceneManager.Instance)
        {
            if (SceneManager.Instance.hasGameOver)
            {
                return;
            }
        }
        UpdateAttackState();
        UpdateFeatureState();
        UpdateSwitchRopeState();
        UpdateHurtSoundState();
        PressPause();
        Aim(gun);
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
        MoveAnimationControl(CharacterAnimation.ShooterLeft_Run,CharacterAnimation.ShooterRight_Run,CharacterAnimation.ShooterLeft_Idle,CharacterAnimation.ShooterRight_Idle);
        WeaponLayerChange();
        UseItem();
        CheckDistanceToBattery();
        //CheckKeys();
        if (playerInputSetting.GetOptionalFeatureDown())
        {
            switch (feature)
            {
                case OptionalFeature.Salvo:
                    Salvo();
                    break;
                case OptionalFeature.DestroyTorpedoes:
                    DestroyTorpedoes();
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
        if(hasDead && other.gameObject.tag=="ReconnectArea")
        {
            SceneManager.Instance.BatteryTransform.GetComponent<BatteryCarrier>().readyToRebornPlayers.Add(this);
            return;
        }

        //�������������UI
        CreatBubbleUI(other.gameObject);

        switch (other.gameObject.tag)
        {
            case "Item":
                MusicManager.Instance.PlaySound("收集");
                Instantiate(Resources.Load<GameObject>("Effect/PickupTaskitem"), other.transform.position, Quaternion.Euler(-90, 0, 0));
                other.GetComponent<Item>().Apply(gameObject);
                break;
            case "ResToCollecting":
                MusicManager.Instance.PlaySound("收集");
                other.gameObject.GetComponent<ResToCollecting>().Collecting();
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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
                        bubbleManager.DestroyBubble();

                    }
                }
                break;
            case "Resource":
                if ( playerInputSetting.GetInteractButtonDown())
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
            case "Portal":
                if (SceneManager.Instance.matchVictoryCondition)
                {
                    if (playerInputSetting.GetInteractButtonDown())
                    {
                        bubbleManager.DestroyBubble();
                        if (!SceneManager.Instance.isSecondLevel)
                        {
                            //载入下一关
                            SceneManager.Instance.mask.DOFade(1, 1f);
                            UnityEngine.SceneManagement.SceneManager.LoadScene("StoreScene");
                        }
                        else
                        {
                            SceneManager.Instance.GameOver(true);
                        }
                    }
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
                bubbleManager.DestroyBubble();
                break;
            case "ReconnectArea":
                bubbleManager.DestroyBubble();
                break;
            case "Chest":
                bubbleManager.DestroyBubble();
                break;
            default:
                break;
        }

    }

    //������
    public override bool MainAttack()
    {
        if(!base.MainAttack())
        {
            return false;
        }
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Effect/FlashSpiky"), shootTransform.position,
            gun.transform.rotation,shootTransform);
        bullet.GetComponent<Bullet>().Init(mainWeapon, gun.transform.forward);
        MusicManager.Instance.PlaySound("射击");
        return true;
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

    //���
    public override bool SecondaryAttack()
    {
        if(!base.SecondaryAttack())
        {
            return false;
        }
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Harpoon"), transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(secondaryWeapon,gun.transform.forward);

        MusicManager.Instance.PlaySound("鱼叉发射");

        return true;
    }

    public override void SetFeatureCD()
    {
        featureCD = feature switch
        {
            OptionalFeature.Salvo => salvoCD,
            OptionalFeature.DestroyTorpedoes => torpedoesCD,
            _ => 1,
        };
    }

    #region 自选功能
    //齐射 发射六枚微型导弹锁定最近的敌人

    public Collider FindNearestEnemy(Collider[] colliders)
    {
        //找最近的敌人
        Collider nearest = colliders[0];
        foreach (var coll in colliders)
        {
            if (Vector3.Distance(coll.transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
            {
                nearest = coll;
            }
        }
        return nearest;
    }
    public void Salvo()
    {
        if (!canUseFeature) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, salvoRange, enemyLayer);
        if (colliders.Length == 0) return;
        Collider nearest = FindNearestEnemy(colliders);
        MusicManager.Instance.PlaySound("齐射");

        canUseFeature = false;
        _featureCDTimer = featureCD;


        GameObject missileCollection = Instantiate(Resources.Load<GameObject>("RocketMissileCollection"), transform.position + new Vector3(1, 0, 0), Quaternion.identity);

        missileCollection.GetComponent<MissileControl>().target = nearest.gameObject;
        missileCollection.GetComponent<MissileControl>().singleMissileDamage  = salveMissileDamage;
        missileCollection.GetComponent<MissileControl>().Init();
        GetComponent<CellBattery>().ChangePower(-salvoPower);
    }


    //毁灭鱼雷
    public void DestroyTorpedoes()
    {
        if (!canUseFeature) return;

        canUseFeature = false;
        _featureCDTimer = featureCD;
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Torpedoes"), gun.transform.position + gun.transform.forward*2, Quaternion.identity);

        float angle = Vector3.Angle(Vector3.right, gun.transform.forward);
        if(gun.transform.forward.y>=0)
        {
            bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        else
        {
            bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
        }

        bullet.GetComponent<Torpedoes>().Init(enemyLayer,playerLayer,torpedoesSpeed,destoryTime,
            torpedoesRange,torpedoesForce,torpedoesDamage, gun.transform.forward);
        GetComponent<CellBattery>().ChangePower(-torpedoesPower);
    }

    #endregion


    public override void AddBuff(UpgradeBuff buff)
    {
        base.AddBuff(buff);
        switch (buff.buffName)
        {
            case "高效修复":
                //脱战后提前一秒维修护盾
                timeToRepairArmor -= 0.5f;
                break;
            case "电磁放大":
                maxArmor += 2;
                break;
            case "追猎":
                secondaryWeapon.attackDamage += 2;
                secondaryWeapon.attackCD -= 0.5f;
                break;
            case "制导":
                salveMissileDamage += 2;
                salveAmount -= 1;
                salvoCD -= 1;
                break;
            case "刀尖舔血":
                speed += 0.5f;
                maxArmor += 3;
                restoreAmount = 0;
                break;
            default:
                break;
        }
    }
}
