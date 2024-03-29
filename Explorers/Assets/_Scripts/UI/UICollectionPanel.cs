using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICollectionPanel : MonoBehaviour
{
    private SceneManager sceneManager;
    private Image _collectionIconImg;
    private Text _collectionNumText;
    private GameObject _check;
    private bool hasInit;
    public CollectionType collectionType;

    [HideInInspector]
    public int currentNum;
    [HideInInspector]
    public int maxNum;

    private void Awake()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();

        _collectionIconImg = transform.GetChild(0).GetComponent<Image>();
        _collectionNumText = transform.GetChild(1).GetComponent<Text>();
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
        maxNum = sceneManager.collectionTasks.Find(x => x.type == collectionType).amount;
        _collectionNumText.text = currentNum.ToString()+"/"+ maxNum.ToString();
        _check.SetActive(false);
    }

    private void SetResUI()
    {
        _collectionNumText.text = currentNum.ToString() + "/" + maxNum.ToString();
        if (currentNum >= maxNum)
        {
            _check.SetActive(true);
        }
    }
}
