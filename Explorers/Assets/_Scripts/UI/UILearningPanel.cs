using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public struct LearnGroup
{
    public Button featureBtn;
    public Sprite spriteToShow;
}
public class UILearningPanel : MonoBehaviour
{
    public LearnGroup[] learnGroup;
    public Image myImg;
    public bool isPause;
    public Button backBtn;
    public Button otherPanelFirst;
    void Start()
    {
        foreach (var group in learnGroup)
        {
            group.featureBtn.onClick.AddListener(()=>OnclickFeatureBtn(group.spriteToShow));
        }
        backBtn.onClick.AddListener(OnClickBackBtn);
    }
    

    private void OnclickFeatureBtn(Sprite sprite)
    {
        myImg.sprite = sprite;
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
        Destroy(gameObject);
    }
}
