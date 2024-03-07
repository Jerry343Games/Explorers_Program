using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionItem : Item
{
    public ResourceType resType;

    public override void Apply(GameObject user)
    {
        //�����ռ�������
        SceneManager.Instance.tasks.Find(x => x.type == resType).taskUI.GetComponent<UIResPanel>().currentNum++;
        Destroy(gameObject);

    }


}
