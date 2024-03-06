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
        if (hasDead) return;
        CharacterMove();
        CheckDistanceToBattery();
    }

    private void OnTriggerStay(Collider other)
    {
        switch(other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if(!_hasConnected /*&& ����������*/)
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
