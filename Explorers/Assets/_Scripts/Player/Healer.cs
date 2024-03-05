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
            //进入可重连绳子区域
            case "ReconnectArea":
                if (_hasConnected /*&& 按下重连键*/)
                {
                    ReconnectRope();
                }

                break;
            default:
                break;
        }
    }

}
