using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreResourcesPanel : MonoBehaviour
{
    public TMP_Text numText;
    public GameObject numBase;

    private int curNum;
    
    public Color endColor = Color.red;  // 结束颜色
    public float duration = 0.01f;  // 持续时间
    private void OnEnable()
    {
        numText.text = PlayerManager.Instance.resNum.ToString();
        curNum = PlayerManager.Instance.resNum;
        CharacterBuffPanel.OnRefreshBtnClick_Can += OnRefresh;
        CharacterBuffPanel.OnRefreshBtnClick_Cant += OnCantRefresh;
    }

    private void OnDisable()
    {
        CharacterBuffPanel.OnRefreshBtnClick_Can -= OnRefresh;
        CharacterBuffPanel.OnRefreshBtnClick_Cant -= OnCantRefresh;
    }

    private void OnRefresh()
    {
        numBase.GetComponent<RectTransform>().DOShakePosition(0.3f, 5f);
        int tar = PlayerManager.Instance.resNum;
        DOTween.To(() => curNum, (value) =>
        {
            numText.text = value.ToString();
        }, tar, 1f).SetEase(Ease.Linear).SetAutoKill(false).SetTarget(this).OnComplete(() => curNum = tar);
    }

    private void OnCantRefresh()
    {
        BlinkColor();
    }
    
    /// <summary>
    /// 颜色闪烁
    /// </summary>
    void BlinkColor()
    {
        // 停止所有当前的动画，防止颜色冲突
        numBase.GetComponent<Image>().DOKill();

        // 当前颜色到闪烁颜色再返回当前颜色
        numBase.GetComponent<Image>().DOColor(endColor, duration)
            .SetLoops(2, LoopType.Yoyo)  
            .SetEase(Ease.Linear);
    }
    
}
