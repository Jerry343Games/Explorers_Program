using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UISelectPanel : MonoBehaviour
{
    [HideInInspector]
    public int currentConfirmPlayer;
    public bool isLaunch;
    public Image mask;
    
    private float _countDown=3f;
    private string _countDownText;
    public SceneManager sceneManager;

    public TMP_Text infoLine1;
    public TMP_Text infoLine2;
    
    public Button battaryBtn;
    public Button shooterBtn;
    public Button fighterBtn;
    public Button healerBtn;

    [HideInInspector]
    public MultiplayerEventSystem battaryEventSystem;
    [HideInInspector]
    public MultiplayerEventSystem shooterEventSystem;
    [HideInInspector]
    public MultiplayerEventSystem fighterEventSystem;
    [HideInInspector]
    public MultiplayerEventSystem healerEventSystem;

    private OptionalFeatureItem _battaryFeature;
    private OptionalFeatureItem _shooterFeature;
    private OptionalFeatureItem _fighterFeature;
    private OptionalFeatureItem _healerFeature;
    public event Action confirmPlayer;
    
    void Start()
    {
        battaryBtn.onClick.AddListener(ClickBattary);
        shooterBtn.onClick.AddListener(ClickShooter);
        fighterBtn.onClick.AddListener(ClickFighter);
        healerBtn.onClick.AddListener(ClickHealer);

        _battaryFeature = battaryBtn.gameObject.GetComponent<CharacterItem>().weaponInfoItem
            .GetComponent<OptionalFeatureItem>();
        _shooterFeature = shooterBtn.gameObject.GetComponent<CharacterItem>().weaponInfoItem
            .GetComponent<OptionalFeatureItem>();
        _fighterFeature = fighterBtn.gameObject.GetComponent<CharacterItem>().weaponInfoItem
            .GetComponent<OptionalFeatureItem>();
        _healerFeature = healerBtn.gameObject.GetComponent<CharacterItem>().weaponInfoItem
            .GetComponent<OptionalFeatureItem>();

         _battaryFeature.FeatureConfirmEvent += LaunchPlayer;
         _shooterFeature.FeatureConfirmEvent += LaunchPlayer;
         _fighterFeature.FeatureConfirmEvent += LaunchPlayer;
         _healerFeature.FeatureConfirmEvent += LaunchPlayer;
    }

    private void Update()
    {
        if (isLaunch)
        {
            if (_countDown>0)
            {
                _countDownText = _countDown.ToString("F2");
                _countDown -= Time.deltaTime;
            }
            else
            {
                _countDownText = "0.00";
            }
            infoLine1.text = "///所有单元已就绪，启动发射程序: "+_countDownText+" ///";    
        }
    }

    private void LaunchPlayer()
    {
        currentConfirmPlayer++;
        Debug.Log(currentConfirmPlayer);
        if (currentConfirmPlayer==sceneManager.maxPlayer)
        {
            Sequence q = DOTween.Sequence();
            infoLine1.rectTransform.DOAnchorPos(new Vector2(infoLine1.rectTransform.anchoredPosition.x, -10), 0.2f);
            infoLine1.rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
            infoLine2.text = null;
            q.AppendInterval(1f);
            isLaunch = true;
            q.Append(battaryBtn.GetComponent<RectTransform>().DOAnchorPos(new Vector2(battaryBtn.GetComponent<RectTransform>().anchoredPosition.x,-600),0.3f));
            q.Append(shooterBtn.GetComponent<RectTransform>().DOAnchorPos(new Vector2(shooterBtn.GetComponent<RectTransform>().anchoredPosition.x,-600),0.3f));
            q.Append(fighterBtn.GetComponent<RectTransform>().DOAnchorPos(new Vector2(fighterBtn.GetComponent<RectTransform>().anchoredPosition.x,-600),0.3f));
            q.Append(healerBtn.GetComponent<RectTransform>().DOAnchorPos(new Vector2(healerBtn.GetComponent<RectTransform>().anchoredPosition.x,-600),0.3f));
            q.AppendInterval(1f);
            q.Append(mask.DOFade(1, 0.3f));
        }
    }
    
    private void ClickBattary()
    {
        battaryEventSystem = MultiplayerEventSystem.current.gameObject.GetComponent<MultiplayerEventSystem>();
        
        confirmPlayer?.Invoke();
    }

    private void ClickShooter()
    {
        shooterEventSystem = MultiplayerEventSystem.current.gameObject.GetComponent<MultiplayerEventSystem>();
        confirmPlayer?.Invoke();
    }

    private void ClickFighter()
    {
        fighterEventSystem = MultiplayerEventSystem.current.gameObject.GetComponent<MultiplayerEventSystem>();
        confirmPlayer?.Invoke();
    }

    private void ClickHealer()
    {
        healerEventSystem = MultiplayerEventSystem.current.gameObject.GetComponent<MultiplayerEventSystem>();
        confirmPlayer?.Invoke();
    }
    
}
