using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionalPanel : MonoBehaviour
{
    public Button backBtn;
    public UIStartPage uiStartPage;
    
    private void Start()
    {
        backBtn.onClick.AddListener(ClickOption);
    }

    private void ClickOption()
    {
        GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        uiStartPage.isOpenPanel = false;
    }
}
