using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionalFeatureItem : MonoBehaviour
{
    public UISelectPanel uiSelectPanel;
    private MultiplayerEventSystem myEventsystem;
    
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

    public Image optionalFeatureImg;
    public Image myFeature1Img;
    public Image myFeature2Img;

    private Color _selectColor = new Color(1, 0.8f, 0.5f, 1);
    private Color _unSelectColor = new Color(1, 0.8f, 0.5f, 0);
    
    public event Action FeatureConfirmEvent;

    public TargetType targetType;

    private bool _hasConfirmed;
    public enum TargetType
    {
        BattaryWeapon,
        ShooterWeapon,
        FigherWeapon,
        HealerWeapon,
    }
    
    void Start()
    {
        _mainWeaponBtn = mainWeapon.GetComponent<Button>();
        _secondaryWeaponBtn = secondaryWeapon.GetComponent<Button>();
        _optionalFeature1Btn = optionalFeature1.GetComponent<Button>();
        _optionalFeature2Btn = optionalFeature2.GetComponent<Button>();
        
        _secondaryWeaponBtn.onClick.AddListener(ClickSecWeapon);
        _optionalFeature1Btn.onClick.AddListener(ClickFeature1);
        _optionalFeature2Btn.onClick.AddListener(ClickFeature2);

        uiSelectPanel.confirmPlayer += ConfirmPlayer;

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
        if (myEventsystem&&!_hasConfirmed)
        {
            switch (myEventsystem.currentSelectedGameObject.name)
            {
                case "MainWeaponInfo":
                    mainWeaponSelectImg.color = _selectColor;
                    secondaryWeaponSelectImg.color = _unSelectColor;
                    optionalFeatureSelectImg.color = _unSelectColor;
                    optionalFeatureImg.sprite = null;
                    break;
                case "SecondaryWeaponInfo":
                    mainWeaponSelectImg.color = _unSelectColor;
                    secondaryWeaponSelectImg.color = _selectColor;
                    optionalFeatureSelectImg.color = _unSelectColor;
                    optionalFeatureImg.sprite = null;
                    break;
                case "OptionalFeature1":
                    mainWeaponSelectImg.color = _unSelectColor;
                    secondaryWeaponSelectImg.color = _unSelectColor;
                    optionalFeatureSelectImg.color = _selectColor;
                    optionalFeatureImg.sprite = myFeature1Img.sprite;
                    break;
                case "OptionalFeature2":
                    mainWeaponSelectImg.color = _unSelectColor;
                    secondaryWeaponSelectImg.color = _unSelectColor;
                    optionalFeatureSelectImg.color = _selectColor;
                    optionalFeatureImg.sprite = myFeature2Img.sprite;
                    break;
            }
        }
    }
    
    /// <summary>
    /// 选择自选功能1
    /// </summary>
    private void ClickFeature1()
    {
        FeatureConfirmEvent?.Invoke();
        optionalFeatureSelectImg.color = _unSelectColor;
        _hasConfirmed = true;
    }

    /// <summary>
    /// 选择自选功能2
    /// </summary>
    private void ClickFeature2()
    {
        FeatureConfirmEvent?.Invoke();
        optionalFeatureSelectImg.color = _unSelectColor;
        _hasConfirmed = true;
    }

    private void ConfirmPlayer()
    {
        switch (targetType)
        {
            case TargetType.BattaryWeapon:
                myEventsystem = uiSelectPanel.battaryEventSystem;
                break;
            case TargetType.ShooterWeapon:
                myEventsystem = uiSelectPanel.shooterEventSystem;
                break;
            case TargetType.FigherWeapon:
                myEventsystem = uiSelectPanel.fighterEventSystem;
                break;
            case TargetType.HealerWeapon:
                myEventsystem = uiSelectPanel.healerEventSystem;
                break;
        }
    }
}
