using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BatteryCarrier : PlayerController
{


    [Header("����")]
    public PlayerController selectedPlayer;//ѡ���Ҫ���в����Ļ�����
    public Image selectPlayerImage;

    public float overloadDuration;//���س���ʱ��
    public float overloadCD;//���ع�����ȴʱ��
    public int overloadPower;//���ع��ܺĵ���
    private bool isOverloading;
    private bool canOverload;
    private float _overloadTimer;
    private float _overloadCDTimer;
    private PlayerController _overloadPlayer;
    private string tempName;

    [Header("�绡���")]
    public float lightningAttackCD;//�绡�����ȴʱ��
    public int targetNumber;
    private float _lightningAttackTimer;
    public int lightningAttackDamage;//�绡����˺�
    public float lightningAttackRange;//�绡�����Χ
    public int lightningAttackPower;//�绡����ĵ���
    private bool canLightningAttack;
    public LayerMask enemyLayer;

    [Header("����")]
    public float chargeCD;//������ȴʱ��
    public int chargePower;//��������ĵ���

    [Header("����")]
    public int logisticsPower;//���ڹ��ܺĵ���
    public float logisticsCD;//���ڹ���CD

    [Header("��Ԯ")]
    public List<PlayerController> readyToRebornPlayers = new List<PlayerController>();
    void Start()
    {
        PlayerInit();
        canOverload = true;
        if(PlayerManager.Instance)
        {
            PlayerManager.Instance.hasMainBattary = true; //֪ͨ��ؼ��룬������ְҵ�����Ի�������
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

    #region ����
    public void Overload()
    {
        if (!canOverload || !selectedPlayer) return;
        isOverloading = true;
        canOverload = false;
        _overloadTimer = overloadDuration;
        _overloadCDTimer = overloadCD;
        _overloadPlayer = selectedPlayer;
        MusicManager.Instance.PlaySound("����");

        GetComponent<MainBattery>().ChangePower(-overloadPower);
        Instantiate(Resources.Load<GameObject>("Effect/SparkYellow"), transform.position, Quaternion.identity);
        GameObject upEffect = Instantiate(Resources.Load<GameObject>("Effect/PowerUp"), selectedPlayer.transform.position, Quaternion.identity);
        upEffect.transform.SetParent(selectedPlayer.transform);
        Destroy(upEffect, overloadDuration);

        tempName = selectedPlayer.gameObject.name;
        switch (selectedPlayer.gameObject.name)
        {
            case "Shooter"://����
                selectedPlayer.mainWeapon.attackDamage *= 1.5f;
                selectedPlayer.secondaryWeapon.attackDamage *= 1.5f;
                Debug.Log("ǿ��������");
                break;
            case "Fighter"://սʿ
                (selectedPlayer as Fighter).tempArmor = selectedPlayer.maxArmor;
                Debug.Log("ǿ����սʿ");
                break;
            case "Healer"://ҽ��
                selectedPlayer.mainWeapon.attackDamage *= 2;
                Debug.Log("ǿ����ҽ��");

                break;
            default:
                break;
        }
    }
    public void BackFormOverload()
    {
        switch (tempName)
        {
            case "Shooter"://����
                selectedPlayer.mainWeapon.attackDamage /= 1.5f;
                selectedPlayer.secondaryWeapon.attackDamage /= 1.5f;
                break;
            case "Fighter"://սʿ
                (selectedPlayer as Fighter).tempArmor = 0;
                break;
            case "Healer"://ҽ��
                selectedPlayer.mainWeapon.attackDamage /= 2;
                break;
            default:
                break;
        }
    }

    #endregion

    #region �绡���
    public void Lightning()
    {
        if (!canLightningAttack) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, lightningAttackRange, enemyLayer);
        if (colliders.Length == 0) return;

        MusicManager.Instance.PlaySound("�绡���");
        canLightningAttack = false;
        _lightningAttackTimer = lightningAttackCD;
        Instantiate(Resources.Load<GameObject>("Effect/SparkBlue"), transform.position, Quaternion.identity);
        int attackCount = Mathf.Min(colliders.Length, targetNumber); // ʵ�ʹ�����Ŀ����
        GetComponent<MainBattery>().ChangePower(-lightningAttackPower * attackCount); // �������ĸ���ʵ�ʹ�����Ŀ��������
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

    #region ����
    public void Charge()
    {
        if (!canUseFeature) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        GetComponent<MainBattery>().ChangePower(-chargePower);
        MusicManager.Instance.PlaySound("����");
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player == this) continue;
            player.currentArmor = player.maxArmor;
        }

    }
    #endregion

    #region ����
    public void Logistics()
    {
        if (!canUseFeature || !selectedPlayer) return;
        canUseFeature = false;
        _featureCDTimer = featureCD;
        GetComponent<MainBattery>().ChangePower(-logisticsPower);
        //MusicManager.Instance.PlaySound("����");

        //û�оͻ�õ��� ���˾��滻ԭ���ĵ���
        int randomItem = Random.Range(0, 3);
        //��ϲ����swith ��õ��� �����У�
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
            case "���ݵ��":
                GetComponent<MainBattery>().maxPower += 200;
                GetComponent<MainBattery>().currentPower += 200;
                break;
            case "����":
                overloadDuration += 1.5f;
                overloadPower += 2;
                break;
            case "���߳�":
                targetNumber += 1;
                break;
            case "���ұ�ѹ��":
                lightningAttackDamage += 1;
                lightningAttackRange -= 0.4f;
                lightningAttackPower -= 1;
                break;
            default:
                break;

        }
    }
}
