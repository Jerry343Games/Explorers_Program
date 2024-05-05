using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIStory : MonoBehaviour
{

    public List<GameObject> objectsToEnable;
    public List<RectTransform> targetPos;
    public float timeBetweenObjects = 1.0f;
    public GameObject ui0;
    private int currentIndex = 0;
    public float fadeTime = 1f;
    private CanvasRenderer canvasRenderer;

    public Image warningMas;

    public RectTransform pages;
    
    private void Start()
    {
        Sequence _sequence = DOTween.Sequence();
        //第一张图淡入
        _sequence.Append(objectsToEnable[0].GetComponent<Image>().DOFade(1, 0.5f));
        _sequence.AppendInterval(timeBetweenObjects);
        //第一张图淡出
        _sequence.Append(objectsToEnable[0].GetComponent<Image>().DOFade(0, 0.5f));
        _sequence.AppendInterval(timeBetweenObjects);
        //第二张图滑入
        _sequence.Append(objectsToEnable[2].GetComponent<RectTransform>().DOAnchorPos(targetPos[1].anchoredPosition, 0.5f)); 
        _sequence.AppendInterval(timeBetweenObjects);
        //第三张图滑入
        _sequence.Append(objectsToEnable[1].GetComponent<RectTransform>().DOAnchorPos(targetPos[0].anchoredPosition, 0.5f).OnComplete(()=>objectsToEnable[1].GetComponent<RectTransform>().DOShakeAnchorPos(0.5f,10f))); 
        _sequence.AppendInterval(timeBetweenObjects);
        //第四张图滑入，警告闪烁
        _sequence.Append(objectsToEnable[3].GetComponent<RectTransform>().DOAnchorPos(targetPos[2].anchoredPosition, 0.5f).OnComplete(()=>warningMas.DOFade(0.05f, 0.3f).SetLoops(10,LoopType.Yoyo))); 
        _sequence.AppendInterval(timeBetweenObjects);
        //第五张图滑入
        _sequence.Append(objectsToEnable[4].GetComponent<RectTransform>().DOAnchorPos(targetPos[3].anchoredPosition, 0.5f));
        _sequence.AppendInterval(timeBetweenObjects+0.5f);
        //整体撤出
        _sequence.Append(pages.DOAnchorPos(new Vector2(0, 600f), 1f));
        _sequence.Append(GetComponent<Image>().DOFade(0, 0.5f)).OnComplete(()=>Destroy(gameObject));
    }

    // void Start()
    // {
    //
    //     DOTween.ToAlpha(() => ui0.GetComponent<Image>().color,
    //             color => ui0.GetComponent<Image>().color = color,
    //             1f, // 目标透明度（0表示完全透明）
    //             3f);
    //
    //     Invoke(nameof(StartUIFade), 4f);
    //     
    // }
    // void StartUIFade()
    // {
    //     // 淡出的代码示例
    //     DOTween.ToAlpha(() => ui0.GetComponent<Image>().color,
    //                     color => ui0.GetComponent<Image>().color = color,
    //                     0f, // 目标透明度（0表示完全透明）
    //                     fadeTime).OnComplete(StartUIAfter);
    // }
    // void StartUIAfter()
    // {
    //     StartCoroutine(EnableObjectsCoroutine());
    // }
    // IEnumerator EnableObjectsCoroutine()
    // {
    //     while (currentIndex < objectsToEnable.Count)
    //     {
    //         objectsToEnable[currentIndex].SetActive(true);
    //         currentIndex++;
    //
    //         yield return new WaitForSeconds(timeBetweenObjects);
    //     }
    // }
    // private void Update()
    // {
    //     if (currentIndex >= objectsToEnable.Count)
    //     {
    //         if (Input.anyKeyDown)
    //         {
    //             foreach(var ui in objectsToEnable)
    //             {
    //                 DOTween.ToAlpha(() => ui.GetComponent<Image>().color,
    //                     color => ui.GetComponent<Image>().color = color,
    //                     0f, // 目标透明度（0表示完全透明）
    //                     fadeTime).OnComplete(DestroyThis);
    //             }
    //             
    //             
    //         }
    //     }
    // }
    // public void DestroyThis()
    // {
    //     Destroy(gameObject);
    // }
}
