using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public int maxPower=10;
    public int decayValue = 1;
    
    //[HideInInspector]
    public int currentPower;
    
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
