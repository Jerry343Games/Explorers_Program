using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionalFeatureItem : MonoBehaviour
{
    public GameObject mainWeapon;
    public GameObject secondaryWeapon;
    public GameObject optionalFeature1;
    public GameObject optionalFeature2;
    
    private Button _mainWeaponBtn;
    private Button _secondaryWeaponBtn;
    private Button _optionalFeature1Btn;
    private Button _optionalFeature2Btn;

    public Image mainWeaponSelectImg;
    public Image secondaryWeaponSelectImg;
    public Image optionalFeatureSelectImg;

    private Color _selectColor = new Color(1, 0.8f, 0.5f, 1);
    private Color _unSelectColor = new Color(1, 0.8f, 0.5f, 0);
    
    public event Action FeatureConfirmEvent;
    
    
    void Start()
    {
        _mainWeaponBtn = mainWeapon.GetComponent<Button>();
        _secondaryWeaponBtn = secondaryWeapon.GetComponent<Button>();
        _optionalFeature1Btn = optionalFeature1.GetComponent<Button>();
        _optionalFeature2Btn = optionalFeature2.GetComponent<Button>();
        
        _secondaryWeaponBtn.onClick.AddListener(ClickSecWeapon);
        _optionalFeature1Btn.onClick.AddListener(ClickFeature1);
        _optionalFeature2Btn.onClick.AddListener(ClickFeature2);
    }

    private void Update()
    {
        SelectWeapon();
    }

    private void ClickMainWeapon()
    {
        
    }

    private void ClickSecWeapon()
    {
        
    }

    private void SelectWeapon()
    {
        switch (MultiplayerEventSystem.current.currentSelectedGameObject.name)
        {
            case "MainWeaponInfo":
                mainWeaponSelectImg.color = _selectColor;
                secondaryWeaponSelectImg.color = _unSelectColor;
                optionalFeatureSelectImg.color = _unSelectColor;
                break;
            case "SecondaryWeaponInfo":
                mainWeaponSelectImg.color = _unSelectColor;
                secondaryWeaponSelectImg.color = _selectColor;
                optionalFeatureSelectImg.color = _unSelectColor;
                break;
            case "OptionalFeature1":
                mainWeaponSelectImg.color = _unSelectColor;
                secondaryWeaponSelectImg.color = _unSelectColor;
                optionalFeatureSelectImg.color = _selectColor;
                break;
            case "OptionalFeature2":
                mainWeaponSelectImg.color = _unSelectColor;
                secondaryWeaponSelectImg.color = _unSelectColor;
                optionalFeatureSelectImg.color = _selectColor;
                break;
        }
    }
    
    private void ClickFeature1()
    {
        FeatureConfirmEvent?.Invoke();
    }

    private void ClickFeature2()
    {
        FeatureConfirmEvent?.Invoke();
    }
}
