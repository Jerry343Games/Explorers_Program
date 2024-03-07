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
            //�����������������
            case "ReconnectArea":
                if (!_hasConnected && playerInputSetting.GetCableButtonDown())
                {
                    ReconnectRope();
                }
                break;
            //�ռ���������Ʒ
            case "Item":
                other.GetComponent<Item>().Apply(gameObject);
                break;
            default:
                break;
        }
    }

}
