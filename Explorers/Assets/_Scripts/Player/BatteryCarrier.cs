using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BatteryCarrier : PlayerController
{


    [Header("����")]
    public PlayerController selectedPlayer;//ѡ���Ҫ���в����Ļ�����

    public float overloadDuration;//���س���ʱ��
    public float overloadCD;//���ع�����ȴʱ��
    public int overloadPower;//���ع��ܺĵ���
    private bool isOverloading;
    private bool canOverload;
    private float _overloadTimer;
    private float _overloadCDTimer;
    private PlayerController _overloadPlayer;

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
    //private float _chargeCDTimer;
    public int chargePower;//��������ĵ���
    //private bool canCharge;

    [Header("����")]
    public int logisticsPower;//���ڹ��ܺĵ���
    public float logisticsCD;//���ڹ���CD
    //private float _logisticsCDTimer;
    //private bool canLogistics;
    void Start()
    {
        PlayerInit();
        canOverload = true;
        PlayerManager.Instance.hasMainBattary = true; //֪ͨ��ؼ��룬������ְҵ�����Ի�������

    }
    void Update()
    {
        if (hasDead) return;
        CharacterMove();
        //CheckKeys();
        TickTime();
        UpdateFeatureState();
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
        MoveAnimationControl(CharacterAnimation.BatteryRun,CharacterAnimation.BatteryWalk);
        if(playerInputSetting.GetSelectCombination()==Vector3.right)
        {
            selectedPlayer = FindObjectsOfType<PlayerController>().ToList().Find(x => x.gameObject.name == "Shooter");
        }
        else if(playerInputSetting.GetSelectCombination() == Vector3.up)
        {
            selectedPlayer = FindObjectsOfType<PlayerController>().ToList().Find(x => x.gameObject.name == "Fighter");
        }
        else if(playerInputSetting.GetSelectCombination() == Vector3.forward)
        {
            selectedPlayer = FindObjectsOfType<PlayerController>().ToList().Find(x => x.gameObject.name == "Healer");
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

        GetComponent<MainBattery>().ChangePower(-overloadPower);
        switch (selectedPlayer.myIndex)
        {
            case 1://����
                selectedPlayer.mainWeapon.attackDamage *= 1.5f;
                selectedPlayer.secondaryWeapons.attackDamage *= 1.5f;
                break;
            case 2://սʿ
                (selectedPlayer as Fighter).tempArmor = selectedPlayer.maxArmor;
                break;
            case 3://ҽ��
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
            case 1://����
                selectedPlayer.mainWeapon.attackDamage /= 1.5f;
                selectedPlayer.secondaryWeapons.attackDamage /= 1.5f;
                break;
            case 2://սʿ
                (selectedPlayer as Fighter).tempArmor = 0;
                break;
            case 3://ҽ��
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
        foreach(var player in FindObjectsOfType<PlayerController>())
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
        //û�оͻ�õ��� ���˾��滻ԭ���ĵ���
        int randomItem = Random.Range(0, 6);
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
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
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
}
