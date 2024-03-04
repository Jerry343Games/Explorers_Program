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
        
        //启用每秒电量衰减
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }
    
    /// <summary>
    /// 供外部获取当前电量
    /// </summary>
    public float GetCurrentPower
    {
        get { return currentPower; }
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
