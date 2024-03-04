using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBattery : MonoBehaviour
{
    public int maxPower=300;
    public int decayValue = 1;
    private int currentPower;

    private void Awake()
    {
        currentPower = maxPower;
        
        //����ÿ�����˥��
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }
    
    /// <summary>
    /// ���ⲿ��ȡ��ǰ����
    /// </summary>
    public float GetCurrentPower
    {
        get { return currentPower; }
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
