using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : PropItem
{
    //public bool hasPicked;//�Ƿ�ʰȡ
    
    public override void Apply(GameObject user)
    {
        user.GetComponent<PlayerController>().item = this;
        gameObject.SetActive(false);
        
    }

    public override void Use(GameObject user)
    {
        CollectionItem[] items = FindObjectsOfType<CollectionItem>();
        Vector3 nearestItemPos = items[0].transform.position;
        if (items.Length == 0) return;
        float curNearestDis = Vector3.Distance(items[0].transform.position, user.transform.position);
        foreach (var item in items)
        {
            if(Vector3.Distance(item.transform.position,user.transform.position)< curNearestDis)
            {
                nearestItemPos = item.transform.position;
                curNearestDis = Vector3.Distance(item.transform.position, user.transform.position);
            }
        }
        Debug.Log(nearestItemPos);
        MusicManager.Instance.PlaySound("����");
        //����һ��ָʾ���UI
        GameObject mySona = Instantiate(Resources.Load<GameObject>("UI/SonaPanel"),Camera.main.WorldToScreenPoint(user.transform.position), Quaternion.identity, GameObject.FindWithTag("Canvas").transform);
        mySona.GetComponent<SonaItem>().Init(user.GetComponent<PlayerController>().playerSprite.transform,nearestItemPos);
        //�������ɵ���
        user.GetComponent<PlayerController>().item = null;
        Destroy(gameObject);
    }

}
