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
        //����ÿ�����˥��
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }

    private void Update()
    {
        //���Ϊ����״̬�����̲���
        if (isConnected)
        {
            FullChargeBattery();
        }
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
    
    /// <summary>
    /// ������ز�������
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
