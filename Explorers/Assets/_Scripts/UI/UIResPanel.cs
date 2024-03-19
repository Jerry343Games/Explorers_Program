using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResPanel : MonoBehaviour
{
    private SceneManager sceneManager;
    private Image _resIconImg;//��Դͼ��
    private Image _resProcessInner;//��Դ�ռ�������
    private bool hasInit;
    public ResourceType resType;

    //[HideInInspector]
    public int currentNum;
    [HideInInspector]
    public int maxNum;

    private void Awake()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<SceneManager>();

        _resIconImg = transform.GetChild(0).GetComponent<Image>();
        _resProcessInner = transform.GetChild(1).GetChild(0).GetComponent<Image>();

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
    }

    private void SetResUI()
    {
        _resProcessInner.fillAmount = (float)currentNum / maxNum;
    }
}
