using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GetPlayerInformation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string name = other.name;
        int index = other.GetComponent<PlayerController>().myIndex;
        float batteryPower;
        //����ǵ�ػ����˶�ȡ�����������
        if (index==(int)PlayerType.BatteryCarrier)
        {
            int value = 20;
            other.GetComponent<BatteryCarrier>().ChangePower(-value);
            batteryPower = other.GetComponent<BatteryCarrier>().CurrentPower;
            Debug.Log("���ٵ�����"+value+"��ǰ������"+batteryPower);
        }

        Debug.Log("������ƣ�" + name + ", ���������" + index );
    }
}
