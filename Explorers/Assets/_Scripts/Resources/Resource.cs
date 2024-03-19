using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    public ResourceType resType;
    [HideInInspector]
    public bool beDigging;
    [SerializeField]
    private float currentProcess;

    public float maxProcess;

    public float speed;

    public Transform bubblePos;
    [SerializeField]
    private bool hasDigged;//是否已经开采过了

    public PlayerController _diger;

    private BoxCollider coll;


    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        currentProcess = 0;
    }

    private void Update()
    {
        if (hasDigged) return;
        if(maxProcess - currentProcess < 0.1f)
        {
            hasDigged = true;
            _diger.DiggingOver();
            _diger = null;
            coll.enabled = false;
            //增加收集物数量
            SceneManager.Instance.resTasks.Find(x => x.type == resType).taskUI.GetComponent<UIResPanel>().currentNum++;
            //Destroy(gameObject);
        }

        if(beDigging)
        {
            currentProcess += Time.deltaTime * speed;
            currentProcess = Mathf.Clamp(currentProcess,0, maxProcess);
        }
    }


    public void SetDiager(PlayerController digger)
    {
        _diger = digger;
    }
}
