using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public int maxPower=10;
    public int decayValue = 1;
    
    //[HideInInspector]
    public int currentPower;
    

    public Battery(int initialPower)
    {
        this.maxPower = initialPower;
    }
    /// <summary>
    /// �����޸ķ���
    /// </summary>
    public virtual void ChangePower(int value)
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
            ChangePower(-1);
        }
    }
}
