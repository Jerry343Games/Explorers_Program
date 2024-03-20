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

    public CellBattery(int initialPower) : base(initialPower)
    {

    }

    private void Awake()
    {
        Init();
        //����ÿ�����˥��
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
        EventCenter.BattaryJoined += FindMainBattary;
    }

    private void Update()
    {
        if(currentPower <=0 && !GetComponent<PlayerController>().hasDead)
        {
            GetComponent<PlayerController>().SetDeadState(true);
        }
    }
    
    /// <summary>
    /// ��ʼ������
    /// </summary>
    private void Init()
    {
        isConnected = true;
        currentPower = maxPower;
        if (!_mainBattery)
        {
            Debug.LogWarning("CellBattery need a MainBattery");
        }
    }

    private void FindMainBattary()
    {
        _mainBattery = GameObject.Find("BatteryCarrier").GetComponent<MainBattery>();
    }

    /// <summary>
    /// ��д����ķ��������ӵ�����ӵ������ʱ���ı��������صĵ�������δ����ʱ���ı�������������
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
    /// ��������״̬
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
    /// ������������õ�ǰȱ�ٵĵ�
    /// </summary>
    public void GetLackPowerFromMain()
    {
        int power = maxPower - currentPower;
        currentPower = maxPower;
        this._mainBattery.ChangePower(-power);
    }
}
