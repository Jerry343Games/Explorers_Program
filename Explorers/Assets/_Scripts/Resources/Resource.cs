using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ResourceType resType;

    public Transform bubblePos;

    public int spawnMineralAmount;//单次开采生成的矿物个数

    public int canMiningTimes;//可以开采的次数

    public int miningDuration;

    private BoxCollider _coll;

    public List<Transform> excavatorTrans = new List<Transform>();

    public BoxCollider entityColl;

    public Animator ani;

    private void Awake()
    {
        _coll = GetComponent<BoxCollider>();
    }

    public void BeginingDigging()
    {
        _coll.enabled = false;
        entityColl.enabled = false;
        MusicManager.Instance.PlaySound("挖掘机");
        foreach (var tran in excavatorTrans)
        {

            GameObject excavator = Instantiate(Resources.Load<GameObject>("Item/Excavator"),tran.position, Quaternion.identity);

            excavator.transform.GetChild(0).GetComponent<Excavator>().Init(this, miningDuration);
        }

        Invoke("CallBrokenAni", 2f);
    }


    private void CallBrokenAni()
    {
        ani.CrossFade("Broken", 0f);
    }
    public void SpawnMinerals()
    {
        canMiningTimes--;
        Vector3 startPos = transform.position+ new Vector3(0, 0.5f, 0);
        for (int i = 0; i < spawnMineralAmount; i++)
        {

            //爆出矿
            GameObject mineral = Instantiate(Resources.Load<GameObject>("Item/" + resType.ToString()), startPos, Quaternion.identity);
            mineral.GetComponent<ResToCollecting>().Init(resType);


            // 计算贝塞尔曲线路径点
            float xOffset = (i - spawnMineralAmount / 2);
            // 计算贝塞尔曲线路径点
            Vector3 midPos_1 = new Vector3(startPos.x + xOffset, startPos.y+0.5f, startPos.z);
            Vector3 midPos_2 = new Vector3(startPos.x + xOffset, startPos.y+0.2f , startPos.z);
            Vector3 endPos = new Vector3(startPos.x + xOffset, startPos.y, startPos.z);

            // 创建贝塞尔曲线路径点数组
            Vector3[] pathPoints = new Vector3[] { startPos, midPos_1, midPos_2, endPos };

            DOTween.To((t) =>
            {
                mineral.transform.position = BesselCurve(pathPoints, t);
            }, 0, 1, 2f);

        }
        MusicManager.Instance.PlaySound("采矿");
        if (canMiningTimes == 0)
        {
            //Destroy(gameObject);
        }
    }


    public Vector3 BesselCurve(Vector3[] pos, float t)
    {
        Vector3[] arr = new Vector3[pos.Length - 1];
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = pos[i] * (1 - t) + pos[i + 1] * t;
            Debug.DrawLine(pos[i], pos[i + 1], Color.red);
        }
        if (arr.Length == 1)
        {
            return arr[0];
        }
        else
        {
            return BesselCurve(arr, t);
        }
    }


}
