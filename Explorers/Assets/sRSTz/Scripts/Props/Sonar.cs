using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : PropItem
{
    //public bool hasPicked;//是否被拾取


    public override void Apply(GameObject user)
    {
        user.GetComponent<PlayerController>().item = this;
        gameObject.SetActive(false);
    }

    public override void Use(GameObject user)
    {
        CollectionItem[] items = FindObjectsOfType<CollectionItem>();
        Vector3 nearestItemPos = Vector3.zero;
        if (items.Length == 0) return;
        float curNearestDis = Vector3.Distance(items[0].transform.position, user.transform.position);
        foreach (var item in items)
        {
            if(Vector3.Distance(item.transform.position,user.transform.position)< curNearestDis)
            {
                nearestItemPos = item.transform.position;
            }
        }

        //生成一个指示标的UI
        Vector3 nearestDir = (nearestItemPos - user.transform.position).normalized;
        Debug.Log(nearestDir);
        //销毁声纳道具
        user.GetComponent<PlayerController>().item = null;
        Destroy(gameObject);
    }

}
