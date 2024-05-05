using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SmallCollectionUI : MonoBehaviour
{
    private RectTransform _target;
    // Start is called before the first frame update

    public void Init(RectTransform target,Sprite icon)
    {
        _target = target;
        GetComponent<Image>().sprite = icon;
        Sequence q = DOTween.Sequence();
        q.Append( GetComponent<RectTransform>().DOAnchorPos(target.localPosition, 0.5f).OnComplete(()=>GetComponent<Image>().enabled=false));
        q.Append(target.DOShakeAnchorPos(0.5f, 7f).OnComplete(()=>Destroy(gameObject)));
    }
}
