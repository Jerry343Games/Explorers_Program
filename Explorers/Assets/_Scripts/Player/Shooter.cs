using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Serialization;

public class Shooter : PlayerController
{
    private bool isLeft;
    public GameObject gun;
    public Transform shootTransform;
    void Awake()
    {
        PlayerInit();
    }
    // Update is called once per frame
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
        Aim(gun);
        if (playerInputSetting.GetAttackButtonDown())
        {
            Attack();
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
                    _curDigRes.beDingging = true;
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
                    _curDigRes.beDingging = false;
                    _curDigRes = null;
                }
                //�뿪��Դ��������ٽ�������
                bubblePanel.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "ReconnectArea":
                //�뿪���������������������ݾ�������
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
}
