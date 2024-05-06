using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ElectorContol : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPos = transform.localScale;
        transform.localScale = Vector3.zero;
        Sequence q = DOTween.Sequence();
        q.Append(transform.DOScale(startPos, 0.3f));
        q.AppendInterval(1f);
        q.Append(transform.DOScale(Vector3.zero, 0.3f)).OnComplete(() =>
        {
            Destroy(gameObject);
        });

    }
}
