using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Serialization;

public class Shooter : PlayerController
{
    private bool isLeft;
    public GameObject gun;
    public Transform shootTransform;
    [Header("齐射")]
    public float salvoCD;//齐射冷却时间
    private float _salvoCDTimer;
    public int salveAmount;//齐射的导弹数量
    public int salveMissileDamage;//单枚导弹伤害
    public float salvoRange;//齐射检测范围
    public float salvoMissileSpeed;//单枚导弹速度
    private bool canSalvo;
    public LayerMask enemyLayer;

    void Awake()
    {
        PlayerInit();
    }
    // Update is called once per frame
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
        TimeTick();
        Aim(gun);
        if (playerInputSetting.GetAttackButtonDown())
        {
            Attack();
            //Salvo();
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
        //�������������UI
        CreatBubbleUI(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if (!_hasConnected)
                {
                    if (playerInputSetting.GetCableButtonDown())
                    {
                        ReconnectRope();
                        
                        //����������������ʾ����
                        bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                    }
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
                bubblePanel.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "ReconnectArea":
                if (bubblePanel.reconnectCableBuffer)
                {
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();    
                }
                break;
        }

    }

    //������
    public override void MainAttack()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Effect/FlashSpiky"), shootTransform.position,
            gun.transform.rotation);
        bullet.GetComponent<Bullet>().Init(mainWeapon, gun.transform.forward);
    }

    //���
    public override void SecondaryAttack()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(secondaryWeapons,gun.transform.forward);
    }

    #region 自选功能
    //齐射 发射六枚微型导弹锁定最近的敌人
    public void Salvo()
    {
        if (isDigging) return;
        if (!canSalvo) return;
        Collider[] colliders = Physics.OverlapSphere(transform.position, salvoRange, enemyLayer);
        if (colliders.Length == 0) return;
        canSalvo = false;
        //找最近的敌人
        Collider nearest = colliders[0];
        foreach (var coll in colliders)
        {
            if (Vector3.Distance(coll.transform.position, transform.position) < Vector3.Distance(nearest.transform.position, transform.position))
            {
                nearest = coll;
            }
        }
        Debug.Log(salveAmount);
        for (int i = 0;i<salveAmount;i++)
        {
            GameObject missile = Instantiate(Resources.Load<GameObject>("Missile"),
                transform.position + new Vector3(-2 + (4f / salveAmount) * i,2,0),
                Quaternion.Euler(new Vector3(0, 0, -45 - (90f / salveAmount) * i)));

            missile.GetComponent<Missile>().Init(salveMissileDamage,salvoMissileSpeed, nearest.gameObject);
        }
    }

    #endregion

    public void TimeTick()
    {
        if(!canSalvo)
        {
            _salvoCDTimer -= Time.deltaTime;
            if(_salvoCDTimer<0)
            {
                canSalvo = true;
                _salvoCDTimer = salvoCD;
            }
        }
    }
}
