using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIBubbleItem : MonoBehaviour
{
    public TMP_Text contentText;
    
    [HideInInspector]
    public GameObject gameObject1;
    [HideInInspector]
    public GameObject gameObject2;
    [HideInInspector]
    public string instruction;

    private Tweener _tweener;

    public void Init()
    {
        contentText.text = instruction;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f);
        transform.parent = GameObject.FindWithTag("BubbleManager").transform;
    }

    private void Update()
    {
        SetPosition(gameObject1,gameObject2);
    }

    /// <summary>
    /// 在玩家和需要交互的物品的位置间设置该气泡
    /// </summary>
    /// <param name="obj1"></param>
    /// <param name="obj2"></param>
    public void SetPosition(GameObject obj1,GameObject obj2 )
    {
        Vector3 pos = (obj1.transform.position + obj2.transform.position) / 2;
        transform.position = Camera.main.WorldToScreenPoint(pos);
    }

    public void DestoryBubble()
    {
        _tweener = transform.DOScale(0, 0.2f);
        StartCoroutine(WaitToDestory());
    }

    private IEnumerator WaitToDestory()
    {
        yield return _tweener.WaitForCompletion();
        Destroy(gameObject);
    }
}
