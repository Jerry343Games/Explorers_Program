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
        //����ÿ�����˥��
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }

    private void Update()
    {

    }
    
    /// <summary>
    /// ��ʼ������
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
    /// ��������״̬
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeConnectState(bool newState)
    {
        isConnected = newState;
    }
    
}
