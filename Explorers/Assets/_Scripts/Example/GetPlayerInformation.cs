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
        
        //如果是电池机器人读取其电量并减少
        if (index==(int)PlayerType.BatteryCarrier)
        {
            int value = 20;
            other.GetComponent<MainBattery>().ChangePower(-value);
            batteryPower = other.GetComponent<MainBattery>().GetCurrentPower;
            Debug.Log("减少电量："+value+"当前电量："+batteryPower);
        }

        Debug.Log("玩家名称：" + name + ", 玩家索引：" + index );
    }
}
