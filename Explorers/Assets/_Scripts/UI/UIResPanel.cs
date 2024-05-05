using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResPanel : MonoBehaviour
{
    private SceneManager sceneManager;
    private Image _resIconImg;//��Դͼ��
    private bool hasInit;
    public ResourceType resType;

    [HideInInspector]
    public int currentNum;
    [HideInInspector]
    public int maxNum;

    private Text _resProcessText;

    private Vector2 _startPos;

    
    private void Awake()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();

        _resIconImg = transform.GetChild(0).GetComponent<Image>();

        _resProcessText = transform.GetChild(2).GetComponent<Text>();
        _startPos = _resIconImg.rectTransform.anchoredPosition;
    }
    private void Update()
    {
        if (sceneManager.isMaxPlayer && !hasInit)
        {
            Init(); //�����Ҫ������ռ���ɶ���ٸ� ���ڹ̶�
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

    public void AddNum(int num,Transform myTrans)
    {
        currentNum += num;
        GameObject smallUI = Instantiate(Resources.Load<GameObject>("UI/UISmallCollection"),Camera.main.WorldToScreenPoint(myTrans.position),Quaternion.identity,transform);
        smallUI.GetComponent<SmallCollectionUI>().Init(_resIconImg.rectTransform,_resIconImg.sprite,_startPos);
    }
}
