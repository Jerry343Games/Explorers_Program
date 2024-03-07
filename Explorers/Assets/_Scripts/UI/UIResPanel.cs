using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResPanel : MonoBehaviour
{
    private SceneManager sceneManager;
    private Image _resIconImg;
    private Text _resNumText;
    private GameObject _check;
    private bool hasInit;
    public ResourceType resType;

    [HideInInspector]
    public int currentNum;
    [HideInInspector]
    public int maxNum;

    private void Awake()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();

        _resIconImg = transform.GetChild(0).GetComponent<Image>();
        _resNumText = transform.GetChild(1).GetComponent<Text>();
        _check = transform.GetChild(2).gameObject;

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
        maxNum = sceneManager.tasks.Find(x => x.type == resType).amount;
        _resNumText.text = currentNum.ToString()+"/"+ maxNum.ToString();
        _check.SetActive(false);
    }

    private void SetResUI()
    {
        _resNumText.text = currentNum.ToString() + "/" + maxNum.ToString();
        if (currentNum >= maxNum)
        {
            _check.SetActive(true);
        }
    }
}
