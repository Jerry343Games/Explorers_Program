using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem.iOS;

public class UIBubbleItem : MonoBehaviour
{
    public TextMeshProUGUI contentText;
    
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
    }

    /// <summary>
    /// ����Һ���Ҫ��������Ʒ��λ�ü����ø�����
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
