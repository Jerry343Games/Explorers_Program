using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHealthPanel : MonoBehaviour
{
    //private SceneManager sceneManager;
    public Image HealthInner;
    public Image ArmorInner;
    public Image FeatureInner;//功能图片UI
    public Image ItemInner;//道具图片UI
    private Battery _battery;
    private PlayerController _playerController;

    private Image _hitMask;
    private Image _hitShieldMask;
    
    //生命值和盾面板
    public RectTransform healthPanel;
    public RectTransform armorPanel;

    private bool hasInit;

    private Vector2 _healthPanelStartPos;
    private Vector2 _ArmorPanelStartPos;

    private void OnEnable()
    {
        _hitMask = GameObject.FindWithTag("HitMask").GetComponent<Image>();
        _hitShieldMask = GameObject.FindWithTag("HitShieldMask").GetComponent<Image>();
        _healthPanelStartPos = healthPanel.anchoredPosition;
        _ArmorPanelStartPos = armorPanel.anchoredPosition;
    }

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
            case "BatteryCarrierPanel(Clone)":
                GameObject BatteryCarrier= GameObject.Find("BatteryCarrier");
                if (BatteryCarrier)
                {
                    _battery = BatteryCarrier.GetComponent<Battery>();
                    _playerController = BatteryCarrier.GetComponent<PlayerController>();
                    FeatureInner.sprite = Resources.Load<Sprite>("UI/Image/" + BatteryCarrier.GetComponent<PlayerController>().feature.ToString());

                }
                break;
            case "ShooterPanel(Clone)":
                GameObject Shooter = GameObject.Find("Shooter");
                if (Shooter)
                {
                    _battery = Shooter.GetComponent<Battery>();
                    _playerController = Shooter.GetComponent<PlayerController>();
                    FeatureInner.sprite = Resources.Load<Sprite>("UI/Image/" + Shooter.GetComponent<PlayerController>().feature.ToString());

                }
                break;
             case "HealerPanel(Clone)":
                GameObject Healer = GameObject.Find("Healer");
                if (Healer)
                {
                    _battery = Healer.GetComponent<Battery>();
                    _playerController = Healer.GetComponent<PlayerController>();
                    FeatureInner.sprite = Resources.Load<Sprite>("UI/Image/" + Healer.GetComponent<PlayerController>().feature.ToString());

                }
                break;
             case "FighterPanel(Clone)":
                GameObject Fighter = GameObject.Find("Fighter");
                if (Fighter)
                {
                    _battery = Fighter.GetComponent<Battery>();
                    _playerController = Fighter.GetComponent<PlayerController>();
                    FeatureInner.sprite = Resources.Load<Sprite>("UI/Image/" + Fighter.GetComponent<PlayerController>().feature.ToString());

                }
                break;
        }

        _playerController.OnShieldDamage += ShakeArmor;
        _playerController.OnHealthDamage += ShakeHealth;

    }

    private void OnDestroy()
    {
        _playerController.OnShieldDamage -= ShakeArmor;
        _playerController.OnHealthDamage -= ShakeHealth;
    }

    public void ShakeArmor()
    {
        armorPanel.anchoredPosition = _ArmorPanelStartPos;
        Sequence q = DOTween.Sequence();
        q.Append(_hitShieldMask.DOFade(1, 0.2f));
        q.Append(_hitShieldMask.DOFade(0, 0.2f));
        armorPanel.DOShakeAnchorPos(0.3f, 5f).OnComplete(()=>armorPanel.anchoredPosition = _ArmorPanelStartPos);
        Debug.Log("HIt armor");
    }

    public void ShakeHealth()
    {
        healthPanel.anchoredPosition = _healthPanelStartPos;
        Sequence q = DOTween.Sequence();
        q.Append(_hitMask.DOFade(1, 0.2f));
        q.Append(_hitMask.DOFade(0, 0.2f));
        healthPanel.DOShakeAnchorPos(0.3f, 5f).OnComplete(()=>healthPanel.anchoredPosition=_healthPanelStartPos);
        Debug.Log("HIt health");
    }

    private void SetHealthUI()
    {
        if (_battery)
        {
            HealthInner.DOFillAmount((float)_battery.currentPower / _battery.maxPower, 0.2f);
            if(ArmorInner)
            {
                ArmorInner.DOFillAmount((float)_playerController.currentArmor / _playerController.maxArmor, 0.2f);
            }
            if (FeatureInner)
            {
                FeatureInner.transform.GetChild(0).GetComponent<Image>().fillAmount = Mathf.Clamp(_playerController._featureCDTimer / _playerController.featureCD, 0f, 1f);
            }
            if(ItemInner)
            {
                if (_playerController.item)
                {
                    ItemInner.color = Color.white;
                    ItemInner.sprite = Resources.Load<Sprite>("UI/Image/" + (_playerController.item as PropItem).propType.ToString());
                }
                else
                {
                    ItemInner.color = new Color(1, 1, 1, 0);
                }
            }
        }
    }
}
