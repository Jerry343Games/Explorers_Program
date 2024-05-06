using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UISelectPanel : MonoBehaviour
{
    private Vector3 _line1EndValue = new (1.2f, 1.2f, 1.2f);//提示文字终点位置
    private float _line1AniDuration = 0.2f;//文字缩放平移动画时间
    private float _btnEndAnchorPosY = -600f;//按钮终点位置Y坐标
    private float _btnAniDuration = 0.3f;//按钮离开动画时间
    
    [HideInInspector]
    public int currentConfirmPlayer;
    public bool isLaunch;
    private bool _hasBattary;
    public Image mask;
    
    private float _countDown=7.8f;
    private string _countDownText;
    //public SceneManager sceneManager;

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

            if (_countDown<1.5)
            {
                
            }
            infoLine1.text = "///所有单元已就绪，启动发射程序: "+_countDownText+" ///";    
            
        }
    }

    /// <summary>
    /// 发射玩家程序
    /// </summary>
    private void LaunchPlayer()
    {
        currentConfirmPlayer++;
        
        if (currentConfirmPlayer==PlayerManager.Instance.maxPlayerCount)
        {
            Debug.Log("currentConfirmPlayer");
            Debug.Log(_hasBattary);
            if (!_hasBattary)
            {
                infoLine1.rectTransform.DOAnchorPos(new Vector2(infoLine1.rectTransform.anchoredPosition.x, -10), 0.2f);
                infoLine1.rectTransform.DOScale(_line1EndValue, 0.2f);
                infoLine1.text = "未选择电池单元，即将重新载入";
                infoLine2.text = null;
                Invoke("ReloadScene",1f);
            }
            else
            {
                MusicManager.Instance.PlaySound("角色界面发射");
                EventCenter.CallGameStartedEvent();
                Sequence q = DOTween.Sequence();
                infoLine1.rectTransform.DOAnchorPos(new Vector2(infoLine1.rectTransform.anchoredPosition.x, -10), 0.2f);
                infoLine1.rectTransform.DOScale(_line1EndValue, 0.2f);
                infoLine2.text = null;
                q.AppendInterval(4f);
                isLaunch = true;
                q.Append(battaryBtn.GetComponent<RectTransform>()
                    .DOAnchorPos(new Vector2(battaryBtn.GetComponent<RectTransform>().anchoredPosition.x, _btnEndAnchorPosY), _btnAniDuration));
                q.Append(shooterBtn.GetComponent<RectTransform>()
                    .DOAnchorPos(new Vector2(shooterBtn.GetComponent<RectTransform>().anchoredPosition.x, _btnEndAnchorPosY), _btnAniDuration));
                q.Append(fighterBtn.GetComponent<RectTransform>()
                    .DOAnchorPos(new Vector2(fighterBtn.GetComponent<RectTransform>().anchoredPosition.x, _btnEndAnchorPosY), _btnAniDuration));
                q.Append(healerBtn.GetComponent<RectTransform>()
                    .DOAnchorPos(new Vector2(healerBtn.GetComponent<RectTransform>().anchoredPosition.x, _btnEndAnchorPosY), _btnAniDuration));
                MusicManager.Instance.musicAudio.DOFade(0, 2f);
                q.AppendInterval(2f);
                q.Append(mask.DOFade(1, _btnAniDuration)).OnComplete(() =>
                {
                    //完成动画回调
                    UnityEngine.SceneManagement.SceneManager.LoadScene("JerryTest");
                    
                });
            }
        }
    }
    
    /// <summary>
    /// 按下按钮时,获得当前操作按下按钮的事件系统
    /// </summary>
    private void ClickBattary()
    {

        battaryEventSystem = MultiplayerEventSystem.current.gameObject.GetComponent<MultiplayerEventSystem>();
        _hasBattary = true;
        //通知已经赋值，可以获取事件系统
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

    /// <summary>
    /// 重载关卡
    /// </summary>
    private void ReloadScene()
    {
        //手动销毁玩家
        PlayerManager.Instance.DestoryAllPlayers();
        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectScene");
    }
}
