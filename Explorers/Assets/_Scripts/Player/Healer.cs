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
        CharacterMove();
        CheckDistanceToBattery();

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
            default:
                break;
        }
    }

}
