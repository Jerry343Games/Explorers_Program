using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBattery : Battery
{
    public MainBattery(int initialPower) : base(initialPower)
    {
    }
    private void Awake()
    {
        currentPower = maxPower;
        
        //����ÿ�����˥��
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }
    
}
