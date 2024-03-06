using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CellBattery : Battery
{
    private MainBattery _mainBattery;
    private int myPower;
    public bool isConnected;

    public CellBattery(int initialPower) : base(initialPower)
    {

    }

    private void Awake()
    {
        Init();
        //启用每秒电量衰减
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }

    private void Update()
    {
        if(currentPower <=0 && !GetComponent<PlayerController>().hasDead)
        {
            GetComponent<PlayerController>().SetDeadState(true);
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
    /// 重写基类的方法。在子电池连接到主电池时，改变的是主电池的电量；当未连接时，改变的是自身电量。
    /// </summary>
    /// <param name="value"></param>
    public override void ChangePower(int value)
    {
        if (this.isConnected&&this._mainBattery)
        {
            this._mainBattery.ChangePower(value);
        }
        else
        {
            base.ChangePower(value);
        }
    }

    /// <summary>
    /// 设置连接状态
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeConnectState(bool newState)
    {
        isConnected = newState;
    }
    
}
