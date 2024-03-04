using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBattery : MonoBehaviour
{
    public int maxPower=10;
    public int decayValue = 1;
    private int currentPower;
    private MainBattery _mainBattery;
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
        currentPower = maxPower;
        _mainBattery = GameObject.Find("BatteryCarrier").GetComponent<MainBattery>();
        if (!_mainBattery)
        {
            Debug.LogWarning("CellBattery need a MainBattery");
        }
    }
    
    /// <summary>
    /// 供外部获取当前电量
    /// </summary>
    public float GetCurrentPower
    {
        get { return currentPower; }
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
    
    /// <summary>
    /// 电量修改方法
    /// </summary>
    public void ChangePower(int value)
    {
        currentPower += value;
    }

    /// <summary>
    /// 电量衰减方法
    /// </summary>
    private void PowerDecayPreSecond()
    {
        currentPower -= decayValue;
    }
}
