using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMaskShow : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Image>().color=Color.black;
        GetComponent<Image>().DOFade(0, 0.5f);
    }

    public void Close()
    {
        GetComponent<Image>().DOFade(1, 0.5f);
    }
}
