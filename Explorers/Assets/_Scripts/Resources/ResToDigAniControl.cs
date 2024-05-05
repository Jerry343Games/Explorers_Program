using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResToDigAniControl : MonoBehaviour
{
     private float _cutoffHeight;
     public SkinnedMeshRenderer mySkin;
     public Material brokenMat;

     private bool _hasBroken;
     
     /// <summary>
     /// 矿破碎
     /// </summary>
     public void Broken()
     {
          Instantiate(Resources.Load<GameObject>("Effect/RocketExplosion"),transform.position,Quaternion.identity);
          CameraTrace.instance.CameraShake(0.2f,0.7f);
     }

     
     /// <summary>
     /// 矿消失
     /// </summary>
     public void FadeOut()
     {
          mySkin.material = brokenMat;
          brokenMat.SetFloat("_CutoffHeight",1f);
          _hasBroken = true;
          DOTween.To(() =>_cutoffHeight, x => _cutoffHeight=x, -1f, 2f).OnComplete(DestoryThis);
     }

     private void Update()
     {
          if (_hasBroken)
          {
               brokenMat.SetFloat("_CutoffHeight",_cutoffHeight);
          }
     }

     private void DestoryThis()
     {
          Destroy(gameObject);
     }
}
