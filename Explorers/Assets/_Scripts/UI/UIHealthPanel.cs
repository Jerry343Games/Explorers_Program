using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHealthPanel : MonoBehaviour
{
    public Image Inner;
    private CellBattery _cellBattery;
    private MainBattery _mainBattery;

    private void Awake()
    {
        switch (transform.name)
        {
            case "ShooterPanel":
                _cellBattery = GameObject.Find("Shooter").GetComponent<CellBattery>();
                break;
            case "HealerPanel":
                _cellBattery = GameObject.Find("Healer").GetComponent<CellBattery>();
                break;
            case "BatteryCarrier":
                //_cellBattery = GameObject.Find("BatteryCarrier").GetComponent<>();
                break;
            case "Fighter":
                _cellBattery = GameObject.Find("Fighter").GetComponent<CellBattery>();
                break;
        }
    }

    private void Update()
    {
        
    }

    private void SetHealthUI()
    {
        
    }
}
