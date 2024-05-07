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
    
    void Start()
    {
        foreach (var group in learnGroup)
        {
            group.featureBtn.onClick.AddListener(()=>OnclickFeatureBtn(group.spriteToShow));
        }
    }
    

    private void OnclickFeatureBtn(Sprite sprite)
    {
        myImg.sprite = sprite;
    }
}
