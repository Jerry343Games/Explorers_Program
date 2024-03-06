using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : PlayerController
{

    void Awake()
    {
        PlayerInit();
    }
    // Update is called once per frame
    void Update()
    {
        CharacterMove();
        CheckDistanceToBattery();
        //DynamicChangeLengthOfRope();

    }
    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if (_hasConnected /*&& ����������*/)
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
