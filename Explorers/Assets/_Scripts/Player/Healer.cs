using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : PlayerController
{

    void Awake()
    {
        PlayerInit();
    }
    void Update()
    {
        if (hasDead) return;
        CharacterMove();
        CheckDistanceToBattery();

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
                }
                break;
            //收集到场景物品
            case "Item":
                other.GetComponent<Item>().Apply(gameObject);
                break;
            default:
                break;
        }
    }

}
