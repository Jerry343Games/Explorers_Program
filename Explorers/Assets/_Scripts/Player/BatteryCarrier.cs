using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

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
    private float _chargeCDTimer;
    public int chargePower;//��������ĵ���
    private bool canCharge;
    void Start()
    {
        PlayerInit();
        canOverload = true;
        SceneManager.Instance.hasMainBattary = true; //֪ͨ��ؼ��룬������ְҵ�����Ի�������
        
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

    #region ����
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

    #region ����
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
