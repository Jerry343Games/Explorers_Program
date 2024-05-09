using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public float maxPower=10;
    //public float decayValue = 1;
    
    //[HideInInspector]
    public float currentPower;

    public float decayPowerPreSecond = 1;

    public Battery(int initialPower)
    {
        this.maxPower = initialPower;
    }
    /// <summary>
    /// �����޸ķ���
    /// </summary>
    public virtual void ChangePower(float value)
    {
        currentPower = Mathf.Clamp(currentPower+value,0, maxPower);
    }

    /// <summary>
    /// ����˥������
    /// </summary>
    private void PowerDecayPreSecond()
    {
        if(SceneManager.Instance)
        {
            ChangePower(-decayPowerPreSecond);
        }
    }
}
