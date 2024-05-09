using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CellBattery : Battery
{
    private MainBattery _mainBattery;
    private int myPower;
    public bool isConnected;

    private bool _findMainbattary;

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

        if (PlayerManager.Instance.hasMainBattary&&!_findMainbattary)
        {
            FindMainBattary();
            _findMainbattary = true;
        }
    }
    
    /// <summary>
    /// 初始化方法
    /// </summary>
    private void Init()
    {
        isConnected = true;
        _findMainbattary = false;
        currentPower = maxPower;
    }

    private void FindMainBattary()
    {
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
    public override void ChangePower(float value)
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
        if(isConnected)
        {
            GetLackPowerFromMain();
        }
    }


    /// <summary>
    /// 从主电池那里获得当前缺少的电
    /// </summary>
    public void GetLackPowerFromMain()
    {
        float power = maxPower - currentPower;
        currentPower = maxPower;
        this._mainBattery.ChangePower(-power);
    }
}
