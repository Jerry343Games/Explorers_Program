using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIStory : MonoBehaviour
{

    public List<GameObject> objectsToEnable;
    public float timeBetweenObjects = 1.0f;
    public GameObject ui0;
    private int currentIndex = 0;
    public float fadeTime = 1f;
    private CanvasRenderer canvasRenderer;
    void Start()
    {

        DOTween.ToAlpha(() => ui0.GetComponent<Image>().color,
                color => ui0.GetComponent<Image>().color = color,
                1f, // 目标透明度（0表示完全透明）
                3f);

        Invoke(nameof(StartUIFade), 4f);
        
    }
    void StartUIFade()
    {
        // 淡出的代码示例
        DOTween.ToAlpha(() => ui0.GetComponent<Image>().color,
                        color => ui0.GetComponent<Image>().color = color,
                        0f, // 目标透明度（0表示完全透明）
                        fadeTime).OnComplete(StartUIAfter);
    }
    void StartUIAfter()
    {
        StartCoroutine(EnableObjectsCoroutine());
    }
    IEnumerator EnableObjectsCoroutine()
    {
        while (currentIndex < objectsToEnable.Count)
        {
            objectsToEnable[currentIndex].SetActive(true);
            currentIndex++;

            yield return new WaitForSeconds(timeBetweenObjects);
        }
    }
    private void Update()
    {
        if (currentIndex >= objectsToEnable.Count)
        {
            if (Input.anyKeyDown)
            {
                foreach(var ui in objectsToEnable)
                {
                    DOTween.ToAlpha(() => ui.GetComponent<Image>().color,
                        color => ui.GetComponent<Image>().color = color,
                        0f, // 目标透明度（0表示完全透明）
                        fadeTime).OnComplete(DestroyThis);
                }
                
                
            }
        }
    }
    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
