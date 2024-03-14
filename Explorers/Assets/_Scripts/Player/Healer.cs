using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : PlayerController
{
    private bool isLeft = false;
    void Awake()
    {
        PlayerInit();
    }
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
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
        CreatBubbleUI(other.gameObject);
    }
    
    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if (!_hasConnected && playerInputSetting.GetCableButtonDown())
                {
                    ReconnectRope();
                    //����������������ʾ����
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
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

    //��ҽ�ߡ�
    public override void MainAttack()
    {
        foreach(var player in EnemyManager.Instance.players)
        {
            if (player.name == "BatteryCarrier") continue;
            //�ǵ�ؽ�ɫ�ظ�����
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.currentArmor = Mathf.Min(controller.currentArmor + (int)mainWeapon.attackDamage, controller.maxArmor);
        }
    }

    //��֯
    public override void SecondaryAttack()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(secondaryWeapons, new Vector3(transform.localScale.x, 0, 0));
    }

}
