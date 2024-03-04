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
