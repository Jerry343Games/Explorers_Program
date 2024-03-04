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
                _battery = GameObject.Find("BatteryCarrier").GetComponent<Battery>();
                Debug.Log(_battery.transform.name);
                break;
            case "ShooterPanel":
                _battery = GameObject.Find("Shooter").GetComponent<Battery>();
                Debug.Log(_battery.transform.name);
                break;
            // case "HealerPanel":
            //     _battery = GameObject.Find("Healer").GetComponent<Battery>();
            //     break;
            // case "FighterPanel":
            //     _battery = GameObject.Find("Fighter").GetComponent<Battery>();
            //     break;
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
