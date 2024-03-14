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
    private float _lightningAttackTimer;
    public int lightningAttackDamage;//�绡����˺�
    public float lightningAttackRange;//�绡�����Χ
    public int lightningAttackPower;//�绡����ĵ���
    private bool canLightningAttack;
    public LayerMask enemyLayer;


    void Start()
    {
        PlayerInit();
        canOverload = true;
    }
    void Update()
    {
        if (hasDead) return;
        CharacterMove();
        CheckKeys();
        TickTime();

        //��ع��ܲ���
        if (playerInputSetting.GetCableButtonDown())
        {
            Overload();
        }
        if(playerInputSetting.GetAttackButtonDown())
        {
            Lightning();
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
        GetComponent<MainBattery>().ChangePower(-lightningAttackPower);

        int randomEnemyIndex = Random.Range(0, colliders.Length);//���ѡȡһλ���˹���
        float rotationZ = Vector3.Angle((colliders[randomEnemyIndex].transform.position - transform.position).normalized, Vector3.right) * 
            (colliders[randomEnemyIndex].transform.position.y < transform.position.y ? -1 : 1);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
        GameObject lightning = Instantiate(Resources.Load<GameObject>("Lightning"), 
            (colliders[randomEnemyIndex].transform.position+transform.position)/2, rotation);
        lightning.GetComponent<Lightning>().Init(lightningAttackDamage);
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
}
