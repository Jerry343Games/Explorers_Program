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

    public int maxNum;

    private Transform _canvas;

    private Vector2 _startPos;
    
    private void Awake()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();

        _canvas = GameObject.FindWithTag("Canvas").transform;

        _startPos = _collectionIconImg.GetComponent<RectTransform>().anchoredPosition;
    }
    private void Update()
    {
        SetResUI();

    }
    public void Init()
    {
        currentNum = 0;
        maxNum = sceneManager.collectionTasks.Find(x => x.type == collectionType).amount;
        //测试
        //maxNum = 0;
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

    
    public void AddNum(int num,Transform myTrans)
    {
        currentNum+=num;
        GameObject smallUI = Instantiate(Resources.Load<GameObject>("UI/UISmallCollection"),Camera.main.WorldToScreenPoint(myTrans.position),Quaternion.identity,transform);
        smallUI.GetComponent<SmallCollectionUI>().Init(_collectionIconImg.rectTransform,_collectionIconImg.sprite,_startPos);
        Debug.Log("收集到");
    }
}
