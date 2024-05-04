using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICollectionPanel : MonoBehaviour
{
    private SceneManager sceneManager;
    private bool hasInit;
    public CollectionType collectionType;

    public Image _collectionIconImg;
    public Text _collectionNumText;
    public GameObject _check;
    
    [HideInInspector]
    public int currentNum;
    [HideInInspector]
    public int maxNum;

    private void Awake()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();
        

    }
    private void Update()
    {
        SetResUI();

    }
    public void Init()
    {
        currentNum = 0;
        maxNum = sceneManager.collectionTasks.Find(x => x.type == collectionType).amount;
        _collectionIconImg.sprite = Resources.Load<Sprite>("UI/Image/" + collectionType.ToString());
        _collectionNumText.text = currentNum.ToString()+"/"+ maxNum.ToString();
        _check.SetActive(false);
    }

    private void SetResUI()
    {
        _collectionNumText.text = currentNum.ToString() + "/" + maxNum.ToString();
        if (currentNum >= maxNum)
        {
            _check.SetActive(true);
            sceneManager.collectionTasks.Find(x => x.type == collectionType).hasFinshed = true;
        }
    }
}
