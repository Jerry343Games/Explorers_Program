using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionItem : Item
{
    public CollectionType collectionType;

    public override void Apply(GameObject user)
    {
        //�����ռ�������
        SceneManager.Instance.tasks.Find(x => x.type == collectionType).taskUI.GetComponent<UIResPanel>().currentNum++;
        Destroy(gameObject);

    }


}
