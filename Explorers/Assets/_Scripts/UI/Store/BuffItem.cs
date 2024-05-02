using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffItem : MonoBehaviour
{
    [HideInInspector]
    public UpgradeBuff myBuff;

    public bool isSelected;

    public bool isEndOneRound;//标记一轮选择结束，外部重置
    
    public Image icon;
    public TMP_Text name;
    public TMP_Text description;
    public GameObject chooseMask;
    public GameObject unChooseMask;

    private Button _myBtn;

    private Vector2 _maskStartPos=new Vector2(0,-71f);
    private Vector2 _maskEndPos = new Vector2(0,0f);
    
    public event Action chooseBuff;
    
    private void OnEnable()
    {
        _myBtn = GetComponent<Button>();
        _myBtn.onClick.AddListener(OnClickThisBuff);
    }
    
    public void EnableRefresh()
    {
        name.text = myBuff.buffName;
        description.text = myBuff.description;
        icon.sprite = myBuff.buffIcon;
    }

    /// <summary>
    /// 点击buff事件
    /// </summary>
    private void OnClickThisBuff()
    {
        if (!isEndOneRound)
        {
            isSelected = true;
            chooseBuff?.Invoke();
        }
    }

    /// <summary>
    /// 显示选择遮罩图像
    /// </summary>
    public void ShowChooseMask()
    {
        chooseMask.GetComponent<RectTransform>().anchoredPosition = _maskStartPos;
        chooseMask.SetActive(true);
        chooseMask.GetComponent<RectTransform>().DOAnchorPos(_maskEndPos, 0.2f);
    }

    /// <summary>
    /// 显示未选择遮罩图像
    /// </summary>
    public void ShowUnChooseMask()
    {
        unChooseMask.GetComponent<RectTransform>().anchoredPosition = _maskStartPos;
        unChooseMask.SetActive(true);
        unChooseMask.GetComponent<RectTransform>().DOAnchorPos(_maskEndPos, 0.2f);
    }

    
    public void RefreshNextRound()
    {
        chooseMask.GetComponent<RectTransform>().anchoredPosition = _maskStartPos;
        unChooseMask.GetComponent<RectTransform>().anchoredPosition = _maskStartPos;
        chooseMask.SetActive(false);
        unChooseMask.SetActive(true);
        isEndOneRound = false;
        isSelected = false;
    }
}
