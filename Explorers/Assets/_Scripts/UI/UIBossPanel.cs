using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBossPanel : MonoBehaviour
{
    public Image bossHealthInner;

    public Image bossArmorInner;

    public CanvasGroup canvasGroup;
    public void ShowPanel()
    {
        canvasGroup.DOFade(1, 1f);
    }


}
