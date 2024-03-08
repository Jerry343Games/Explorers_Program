using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHealthPanel : MonoBehaviour
{
    //private SceneManager sceneManager;
    public Image HealthInner;
    public Image ArmorInner;
    private Battery _battery;
    private PlayerController _playerController;

    private bool hasInit;
    

    private void Update()
    {
        if (SceneManager.Instance.isMaxPlayer&&!hasInit)
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
                    _playerController = BatteryCarrier.GetComponent<PlayerController>();
                }
                break;
            case "ShooterPanel":
                GameObject Shooter = GameObject.Find("Shooter");
                if (Shooter)
                {
                    _battery = Shooter.GetComponent<Battery>();
                    _playerController = Shooter.GetComponent<PlayerController>();
                }
                break;
             case "HealerPanel":
                GameObject Healer = GameObject.Find("Healer");
                if (Healer)
                {
                    _battery = Healer.GetComponent<Battery>();
                    _playerController = Healer.GetComponent<PlayerController>();
                }
                break;
             case "FighterPanel":
                GameObject Fighter = GameObject.Find("Fighter");
                if (Fighter)
                {
                    _battery = Fighter.GetComponent<Battery>();
                    _playerController = Fighter.GetComponent<PlayerController>();
                }
                break;
        }
    }
    
    private void SetHealthUI()
    {
        if (_battery)
        {
            HealthInner.DOFillAmount((float)_battery.currentPower / _battery.maxPower, 0.2f);
            if(ArmorInner)
            ArmorInner.DOFillAmount((float)_playerController.currentArmor / _playerController.maxArmor, 0.2f);
        }
    }
}
