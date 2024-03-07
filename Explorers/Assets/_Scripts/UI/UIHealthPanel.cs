using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHealthPanel : MonoBehaviour
{
    private SceneManager sceneManager;
    public Image Inner;
    private Battery _battery;

    private bool hasInit;
    
    private void Awake()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();
    }

    private void Update()
    {
        if (sceneManager.isMaxPlayer&&!hasInit)
        {
            Init();
            hasInit = true;
        }
        
        
        SetHealthUI();
    }

    private void Init()
    {
        switch (transform.name)
        {
            case "BatteryCarrierPanel":
                GameObject BatteryCarrier= GameObject.Find("BatteryCarrier");
                if (BatteryCarrier)
                {
                    _battery = BatteryCarrier.GetComponent<Battery>();
                }
                break;
            case "ShooterPanel":
                GameObject Shooter = GameObject.Find("Shooter");
                if (Shooter)
                {
                    _battery = Shooter.GetComponent<Battery>();
                }
                break;
             case "HealerPanel":
                GameObject Healer = GameObject.Find("Healer");
                if (Healer)
                {
                    _battery = Healer.GetComponent<Battery>();
                }
                break;
             case "FighterPanel":
                GameObject Fighter = GameObject.Find("Fighter");
                if (Fighter)
                {
                    _battery = Fighter.GetComponent<Battery>();
                }
                break;
        }
    }
    
    private void SetHealthUI()
    {
        if (_battery)
        {
            Inner.DOFillAmount((float)_battery.currentPower / _battery.maxPower, 0.2f);
        }
    }
}
