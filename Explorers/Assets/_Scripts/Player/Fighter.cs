using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : PlayerController
{



    // Start is called before the first frame update
    void Awake()
    {
        PlayerInit();   
    }

    // Update is called once per frame
    void Update()
    {
        CharacterMove();
        CheckDistanceToBattery();
    }

    private void OnTriggerStay(Collider other)
    {
        switch(other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if(_hasConnected /*&& ����������*/)
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
