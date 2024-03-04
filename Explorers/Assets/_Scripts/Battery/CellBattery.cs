using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBattery : Battery
{
    private MainBattery _mainBattery;
    private int myPower;
    public bool isConnected;

    private void Awake()
    {
        Init();
        //启用每秒电量衰减
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }

    private void Update()
    {
        //如果为链接状态则立刻补满
        if (isConnected)
        {
            FullChargeBattery();
        }
    }
    
    /// <summary>
    /// 初始化方法
    /// </summary>
    private void Init()
    {
        isConnected = true;
        currentPower = maxPower;
        _mainBattery = GameObject.Find("BatteryCarrier").GetComponent<MainBattery>();
        if (!_mainBattery)
        {
            Debug.LogWarning("CellBattery need a MainBattery");
        }
    }
    
    /// <summary>
    /// 从主电池补满电量
    /// </summary>
    public void FullChargeBattery()
    {
        if (currentPower<maxPower)
        {
            int difference = maxPower - currentPower;
            _mainBattery.ChangePower(-difference);
            ChangePower(difference);
        }
    }
    
}
