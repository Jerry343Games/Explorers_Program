using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPausePanel : MonoBehaviour
{
    public RectTransform backGround;
    private Image _maskImage;

    public Button continueBtn;
    public Button backToMenuBtn;
    public Button learnBtn;
    
    private void Awake()
    {
        _maskImage = GetComponent<Image>();
        _maskImage.DOFade(0.9f, 0.3f).SetUpdate(true);
        backGround.DOAnchorPos(Vector2.zero, 0.5f).SetUpdate(true);
        
        continueBtn.onClick.AddListener(ClickContinue);
        backToMenuBtn.onClick.AddListener(ClickBackMenu);
        learnBtn.onClick.AddListener(ClickLearnBtn);
    }

    private void ClickContinue()
    {
        backGround.DOAnchorPos(new Vector2(0, 423), 0.5f).SetUpdate(true);
        _maskImage.DOFade(0, 0.3f).SetUpdate(true).OnComplete(() =>
        {
            SceneManager.Instance.ContinueGame();
            Destroy(gameObject);
        });
    }

    private void ClickBackMenu()
    {
        backGround.DOAnchorPos(new Vector2(0, 423), 0.5f).SetUpdate(true).OnComplete(() =>
        {
            SceneManager.Instance.BackToMenu();
        });
        _maskImage.DOFade(1, 0.1f).SetUpdate(true);
    }

    private void ClickLearnBtn()
    {
        GameObject panel = Instantiate(Resources.Load<GameObject>("UI/LearningPanel"),GameObject.FindWithTag("Canvas").transform);
        GameObject firstSelected = panel.GetComponent<UILearningPanel>().learnGroup[0].featureBtn.gameObject;
        panel.GetComponent<UILearningPanel>().isPause = true;
        panel.GetComponent<UILearningPanel>().otherPanelFirst = continueBtn;
        foreach (var player in PlayerManager.Instance.players)
        {
            player.GetComponent<PlayerInputSetting>().SwitchToUISchemeAndSelect(firstSelected);
        }
    }
}
