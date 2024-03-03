using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryCarrier : PlayerController
{
    public float maxPower=300;
    private float currentPower;
    void Start()
    {
        PlayerInit();
        currentPower = maxPower;
    }
    
    // Update is called once per frame
    void Update()
    {
        CharacterMove();
    }
    
    public float CurrentPower
    {
        get { return currentPower; }
    }

    public void ChangePower(float value)
    {
        currentPower += value;
    }
}
