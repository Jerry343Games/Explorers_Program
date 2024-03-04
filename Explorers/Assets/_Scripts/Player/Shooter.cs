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
                if (!_hasConnected && Input.GetKeyDown(KeyCode.E))
                {
                    _hasConnected = true;
                    ReconnectRope();
                }

                break;
            default:
                break;
        }
    }
}
