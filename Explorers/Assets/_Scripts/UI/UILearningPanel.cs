using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct LearnGroup
{
    public Button featureBtn;
    public Sprite spriteToShow;
    public string illustrateText;
}
public class UILearningPanel : MonoBehaviour
{
    public LearnGroup[] learnGroup;
    public Image myImg;
    public TMP_Text myText;
    public bool isPause;
    public Button backBtn;
    public Button otherPanelFirst;
    public RectTransform imgPanel;
    public RectTransform textPanel;
    public RectTransform btnPanel;

    private Vector2 _imgStartPos=new Vector2(0,-322);
    private Vector2 textStartPos = new Vector2(60.763f, -285f);
    private Vector2 textEndPos = new Vector2(60.763f, -165f);
    private Vector2 imgStarttPos = new Vector2(693f, 56f);
    private Vector2 imgEndPos = new Vector2(54f, 56f);
    private Vector2 btnEndPos = new Vector2(-286.5f, 0);
    private Vector2 btnStartPos = new Vector2(-286.5f, 452);
    void Start()
    {
        foreach (var group in learnGroup)
        {
            group.featureBtn.onClick.AddListener(()=>OnclickFeatureBtn(group.spriteToShow,group.illustrateText));
        }
        backBtn.onClick.AddListener(OnClickBackBtn);

        GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetUpdate(true);
        imgPanel.DOAnchorPos(imgEndPos, 0.5f).SetUpdate(true);
        btnPanel.DOAnchorPos(btnEndPos, 0.5f).SetUpdate(true);
        textPanel.DOAnchorPos(textEndPos, 0.5f).SetUpdate(true);

    }
    

    private void OnclickFeatureBtn(Sprite sprite,string text)
    {
        Sequence imgSeq = DOTween.Sequence();
        imgSeq.Append(myImg.rectTransform.DOAnchorPos(_imgStartPos, 0.25f).OnComplete(()=>{myImg.sprite = sprite;})).SetUpdate(true);
        imgSeq.Append(myImg.rectTransform.DOAnchorPos(Vector2.zero, 0.25f)).SetUpdate(true);
        Sequence textSeq = DOTween.Sequence();
        textSeq.Append(myText.DOFade(0, 0.25f).OnComplete(() => myText.text = text)).SetUpdate(true);
        textSeq.Append(myText.DOFade(1, 0.25f)).SetUpdate(true);
    }

    private void OnClickBackBtn()
    {
        if (!isPause)
        {
            PlayerManager.Instance.SwitchEveryPlayerTo("Player");
        }
        else
        {
            PlayerManager.Instance.SwitchAndSelect(otherPanelFirst.gameObject);
        }
        GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetUpdate(true);
        imgPanel.DOAnchorPos(imgStarttPos, 0.5f).SetUpdate(true);
        btnPanel.DOAnchorPos(btnStartPos, 0.5f).SetUpdate(true);
        textPanel.DOAnchorPos(textStartPos, 0.5f).OnComplete(()=>Destroy(gameObject)).SetUpdate(true);
        
        
    }
}
