using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Excavator : MonoBehaviour
{
    public float diggingSpeed;

    private float _diggingTime;

    private float _diggingTimer;

    private Resource _res;

    public Material myMat;
    private float _cutoffHeight;
    

    public void Init(Resource res,float diggingDuration)
    {
        _cutoffHeight = myMat.GetFloat("_CutoffHeight");
        _cutoffHeight = -1;
        _res = res;

        _diggingTime = diggingDuration;

        _diggingTimer = _diggingTime;
        
        DOTween.To(() =>_cutoffHeight, x => _cutoffHeight=x, 1f, 1.5f);
    
    }

    void Update()
    {
        myMat.SetFloat("_CutoffHeight",_cutoffHeight);
        
        if(_diggingTimer>0)
        {
            _diggingTimer -= Time.deltaTime;
        }
        else
        {
            //Destroy(gameObject);
        }
    }
}
