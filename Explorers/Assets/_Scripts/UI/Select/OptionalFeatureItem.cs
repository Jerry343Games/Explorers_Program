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

    public OptionalFeature feature1;
    public OptionalFeature feature2;


    private Color _selectColor = new Color(0, 0f, 0f, 0.2f);
    private Color _unSelectColor = new Color(1, 0.8f, 0.5f, 0);
    
    public event Action FeatureConfirmEvent;

    public PlayerType targetType;

    private bool _hasConfirmed;
    
    
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
                    optionalFeatureImg.color = new Color(0, 0, 0, 0);
                    break;
                case "SecondaryWeaponInfo":
                    mainWeaponSelectImg.color = _unSelectColor;
                    secondaryWeaponSelectImg.color = _selectColor;
                    optionalFeatureSelectImg.color = _unSelectColor;
                    optionalFeatureImg.sprite = null;
                    optionalFeatureImg.color = new Color(0, 0, 0, 0);
                    break;
                case "OptionalFeature1":
                    mainWeaponSelectImg.color = _unSelectColor;
                    secondaryWeaponSelectImg.color = _unSelectColor;
                    optionalFeatureSelectImg.color = _selectColor;
                    optionalFeatureImg.sprite = myFeature1Img.sprite;
                    optionalFeatureImg.color = new Color(1, 1, 1, 1);
                    break;
                case "OptionalFeature2":
                    mainWeaponSelectImg.color = _unSelectColor;
                    secondaryWeaponSelectImg.color = _unSelectColor;
                    optionalFeatureSelectImg.color = _selectColor;
                    optionalFeatureImg.sprite = myFeature2Img.sprite;
                    optionalFeatureImg.color = new Color(1, 1, 1, 1);
                    break;
            }
        }
    }
    
    /// <summary>
    /// ѡ����ѡ����1
    /// </summary>
    private void ClickFeature1()
    {
        SetOptionalFeature(feature1);
        FeatureConfirmEvent?.Invoke();//这个事件是给面板收起和锁定用的，所有信息传递必须在此之前完成
        optionalFeatureSelectImg.color = _unSelectColor;
        _hasConfirmed = true;
    }

    /// <summary>
    /// ѡ����ѡ����2
    /// </summary>
    private void ClickFeature2()
    {
        SetOptionalFeature(feature2);
        FeatureConfirmEvent?.Invoke();//这个事件是给面板收起和锁定用的，所有信息传递必须在此之前完成
        optionalFeatureSelectImg.color = _unSelectColor;
        _hasConfirmed = true;
    }

    private void SetOptionalFeature(OptionalFeature feature)
    {
        if(!PlayerManager.Instance.playerFeaturesDic.ContainsKey((int)targetType))
        {
            PlayerManager.Instance.playerFeaturesDic.Add((int)targetType,feature);
            Debug.Log(targetType + " Choose: " + PlayerManager.Instance.playerFeaturesDic[(int)targetType]);
        }
        else
        {
            PlayerManager.Instance.playerFeaturesDic[(int)targetType] = feature;
            Debug.Log(targetType+" Choose: "+PlayerManager.Instance.playerFeaturesDic[(int)targetType]);
        }
    }
    private void ConfirmPlayer()
    {
        switch (targetType)
        {
            case PlayerType.BatteryCarrier:
                myEventsystem = uiSelectPanel.battaryEventSystem;
                break;
            case PlayerType.Shooter:
                myEventsystem = uiSelectPanel.shooterEventSystem;
                break;
            case PlayerType.Fighter:
                myEventsystem = uiSelectPanel.fighterEventSystem;
                break;
            case PlayerType.Healer:
                myEventsystem = uiSelectPanel.healerEventSystem;
                break;
        }
    }
}
