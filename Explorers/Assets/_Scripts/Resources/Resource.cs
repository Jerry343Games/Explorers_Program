using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    public ResourceType resType;
    [HideInInspector]
    public bool beDingging;

    private float currentProcess;

    public float maxProcess;

    public float speed;

    public Image processImg;

    private void Start()
    {
        currentProcess = 0;
        processImg.DOFillAmount(currentProcess / maxProcess, 0.25f);
    }

    private void Update()
    {
        if(beDingging)
        {
            currentProcess += Time.deltaTime * speed;
            currentProcess = Mathf.Clamp(currentProcess,0, maxProcess);
            processImg.DOFillAmount(currentProcess / maxProcess, 0.25f);
        }
    }
}
