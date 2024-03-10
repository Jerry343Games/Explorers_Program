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

    public Transform bubblePos;

    private void Start()
    {
        currentProcess = 0;
    }

    private void Update()
    {
        if(beDingging)
        {
            currentProcess += Time.deltaTime * speed;
            currentProcess = Mathf.Clamp(currentProcess,0, maxProcess);
        }
    }
}
