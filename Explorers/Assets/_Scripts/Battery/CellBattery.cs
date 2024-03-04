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
        currentPower = maxPower;
        _mainBattery = GameObject.Find("BatteryCarrier").GetComponent<MainBattery>();
        if (!_mainBattery)
        {
            Debug.LogWarning("CellBattery need a MainBattery");
        }
    }
    
    /// <summary>
    /// ���ⲿ��ȡ��ǰ����
    /// </summary>
    public float GetCurrentPower
    {
        get { return currentPower; }
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
    
    /// <summary>
    /// �����޸ķ���
    /// </summary>
    public void ChangePower(int value)
    {
        currentPower += value;
    }

    /// <summary>
    /// ����˥������
    /// </summary>
    private void PowerDecayPreSecond()
    {
        currentPower -= decayValue;
    }
}
