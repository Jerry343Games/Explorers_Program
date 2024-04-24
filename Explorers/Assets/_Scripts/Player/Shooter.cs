using Unity.Mathematics;
using UnityEngine;

public class Shooter : PlayerController
{
    private bool isLeft;
    public GameObject gun;
    public Transform shootTransform;
    [Header("齐射")]
    public float salvoCD;//齐射冷却时间
    //private float _salvoCDTimer;
    public int salveAmount;//齐射的导弹数量
    public int salveMissileDamage;//单枚导弹伤害
    public float salvoRange;//齐射检测范围
    public float salvoMissileSpeed;//单枚导弹速度
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

    }
    // Update is called once per frame
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
        UpdateFeatureState();
        UpdateSwitchRopeState();
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
        MoveAnimationControlTest(CharacterAnimation.ShooterLeft_Run,CharacterAnimation.ShooterRight_Run,CharacterAnimation.ShooterLeft_Idle,CharacterAnimation.ShooterRight_Idle);
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

                other.GetComponent<Item>().Apply(gameObject);
                break;
            case "ResToCollecting":
                MusicManager.Instance.PlaySound("收集");

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
                if ( playerInputSetting.GetInteractButtonDown())
                {
                    other.GetComponent<Resource>().BeginingDigging();
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
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
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
    public void Salvo()
    {
        if (!canUseFeature) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, salvoRange, enemyLayer);
        if (colliders.Length == 0) return;
        MusicManager.Instance.PlaySound("齐射");

        canUseFeature = false;
        _featureCDTimer = featureCD;
        //找最近的敌人
        Collider nearest = colliders[0];
        foreach (var coll in colliders)
        {
            if (Vector3.Distance(coll.transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
            {
                nearest = coll;
            }
        }
        Debug.Log(nearest.gameObject.name);
        for (int i = 0;i<salveAmount;i++)
        {
            GameObject missile = Instantiate(Resources.Load<GameObject>("Item/RocketTrail"),
                transform.position + new Vector3(-2 + (4f / salveAmount) * i, 2,0),
                Quaternion.Euler(new Vector3(0, 0, -45 - (90f / salveAmount) * i)));

            missile.GetComponent<Missile>().Init(salveMissileDamage,salvoMissileSpeed, nearest.gameObject);
        }
    }


    //毁灭鱼雷
    public void DestroyTorpedoes()
    {
        if (!canUseFeature) return;

        canUseFeature = false;
        _featureCDTimer = featureCD;
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Torpedoes"), gun.transform.position + gun.transform.forward*2, Quaternion.identity);
        bullet.GetComponent<Torpedoes>().Init(enemyLayer,playerLayer,torpedoesSpeed,destoryTime,
            torpedoesRange,torpedoesForce,torpedoesDamage, gun.transform.forward);
        GetComponent<CellBattery>().ChangePower(-torpedoesPower);
    }

    #endregion



}
