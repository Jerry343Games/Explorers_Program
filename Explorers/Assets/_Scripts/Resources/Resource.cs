using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    public ResourceType resType;

    public Transform bubblePos;

    public int spawnMineralAmount;//单次开采生成的矿物个数

    public int canMiningTimes;//可以开采的次数
    //[HideInInspector]
    //public bool beDigging;
    //[SerializeField]
    //private float currentProcess;

    //public float maxProcess;

    //public float speed;

    //[SerializeField]
    //private bool hasDigged;//是否已经开采过了

    //public PlayerController _diger;

    //private BoxCollider coll;


    private void Update()
    {
        //if (hasDigged) return;
        //if(maxProcess - currentProcess < 0.1f)
        //{
        //    hasDigged = true;
        //    _diger = null;
        //    coll.enabled = false;
        //    //增加收集物数量
        //    SceneManager.Instance.resTasks.Find(x => x.type == resType).taskUI.GetComponent<UIResPanel>().currentNum++;
        //    //Destroy(gameObject);
        //}

        //if(beDigging)
        //{
        //    currentProcess += Time.deltaTime * speed;
        //    currentProcess = Mathf.Clamp(currentProcess,0, maxProcess);
        //}
    }


    //public void SetDiager(PlayerController digger)
    //{
    //    _diger = digger;
    //}

    public void SpawnMineralCollections()
    {
        canMiningTimes--;
        for(int i = 0; i < spawnMineralAmount; i++)
        {
            //爆出矿
            GameObject mineral = Instantiate(Resources.Load<GameObject>("Item/" + resType.ToString()), transform.position, Quaternion.identity);
            mineral.GetComponent<Rigidbody>().AddForce(new Vector3((i - spawnMineralAmount / 2) * 5, 5, 0),ForceMode.Impulse);
        }
        if(canMiningTimes==0)
        {
            //
            Destroy(gameObject);
        }
    }
}
