using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResPanel : MonoBehaviour
{
    private SceneManager sceneManager;
    private Image _resIconImg;//资源图标
    private bool hasInit;
    public ResourceType resType;

    [HideInInspector]
    public int currentNum;
    [HideInInspector]
    public int maxNum;

    private Text _resProcessText;

    private void Awake()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();

        _resIconImg = transform.GetChild(0).GetComponent<Image>();

        _resProcessText = transform.GetChild(1).GetComponent<Text>();
    }
    private void Update()
    {
        if (sceneManager.isMaxPlayer && !hasInit)
        {
            Init(); //如果需要随机的收集物啥的再搞 现在固定
            hasInit = true;
        }
        SetResUI();

    }
    private void Init()
    {
        currentNum = 0;
        maxNum = sceneManager.resTasks.Find(x => x.type == resType).amount;
        _resIconImg.sprite = Resources.Load<Sprite>("UI/Image/" + resType.ToString());
    }

    private void SetResUI()
    {
        _resProcessText.text = currentNum + "/" + maxNum;
        if (currentNum >= maxNum)
        {
            sceneManager.resTasks.Find(x => x.type == resType).hasFinshed = true;
        }
    }
}
