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

    private bool bossUIShow;
    public void ShowPanel()
    {
        StartCoroutine(ShowPanelAction());
    }

    IEnumerator ShowPanelAction()
    {
        canvasGroup.DOFade(1, 1f);
        //yield return new WaitForSeconds(1f);
        bossHealthInner.DOFillAmount((float)GiantRockCrab.Instance._currentHealth / GiantRockCrab.Instance.maxHealth, 2f);
        bossArmorInner.DOFillAmount(GiantRockCrab.Instance._currentArmor / GiantRockCrab.Instance.maxArmor, 2f);
        yield return new WaitForSeconds(2f);
        bossUIShow = true;
    }

    private void Update()
    {
        if(bossUIShow)
        {
            bossHealthInner.DOFillAmount((float)GiantRockCrab.Instance._currentHealth / GiantRockCrab.Instance.maxHealth,0.25f);
            bossArmorInner.DOFillAmount(GiantRockCrab.Instance._currentArmor / GiantRockCrab.Instance.maxArmor,0.25f);

        }
    }


}
