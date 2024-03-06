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
        
        //启用每秒电量衰减
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }

    private void Update()
    {
        if(currentPower<=0 && !GetComponent<PlayerController>().hasDead)
        {
            GetComponent<PlayerController>().SetDeadState(true);
        }
    }

}
