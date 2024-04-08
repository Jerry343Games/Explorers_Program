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
                    FeatureInner.sprite = Resources.Load<Sprite>("UI/Image/" + BatteryCarrier.GetComponent<PlayerController>().feature.ToString());

                }
                break;
            case "ShooterPanel":
                GameObject Shooter = GameObject.Find("Shooter");
                if (Shooter)
                {
                    _battery = Shooter.GetComponent<Battery>();
                    _playerController = Shooter.GetComponent<PlayerController>();
                    FeatureInner.sprite = Resources.Load<Sprite>("UI/Image/" + Shooter.GetComponent<PlayerController>().feature.ToString());

                }
                break;
             case "HealerPanel":
                GameObject Healer = GameObject.Find("Healer");
                if (Healer)
                {
                    _battery = Healer.GetComponent<Battery>();
                    _playerController = Healer.GetComponent<PlayerController>();
                    FeatureInner.sprite = Resources.Load<Sprite>("UI/Image/" + Healer.GetComponent<PlayerController>().feature.ToString());

                }
                break;
             case "FighterPanel":
                GameObject Fighter = GameObject.Find("Fighter");
                if (Fighter)
                {
                    _battery = Fighter.GetComponent<Battery>();
                    _playerController = Fighter.GetComponent<PlayerController>();
                    FeatureInner.sprite = Resources.Load<Sprite>("UI/Image/" + Fighter.GetComponent<PlayerController>().feature.ToString());

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
