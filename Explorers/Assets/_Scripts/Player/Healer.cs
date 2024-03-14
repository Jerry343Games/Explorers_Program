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
            //进入可重连绳子区域
            case "ReconnectArea":
                if (!_hasConnected && playerInputSetting.GetCableButtonDown())
                {
                    ReconnectRope();
                    //重连后销毁重连提示气泡
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                }
                break;
            //收集到场景物品
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
                //离开资源区域后销毁交互气泡
                bubblePanel.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "ReconnectArea":
                //离开重连区域后如果有重连气泡就销毁下
                if (bubblePanel.reconnectCableBuffer)
                {
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();    
                }
                break;
        }
        
    }

    //“医者”
    public override void MainAttack()
    {
        foreach(var player in EnemyManager.Instance.players)
        {
            if (player.name == "BatteryCarrier") continue;
            //非电池角色回复护盾
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.currentArmor = Mathf.Min(controller.currentArmor + (int)mainWeapon.attackDamage, controller.maxArmor);
        }
    }

    //编织
    public override void SecondaryAttack()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(secondaryWeapons, new Vector3(transform.localScale.x, 0, 0));
    }

}
