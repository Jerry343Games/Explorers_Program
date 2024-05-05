using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionItem : Item
{
    public CollectionType collectionType;

    public override void Apply(GameObject user)
    {
        //�����ռ�������
        SceneManager.Instance.collectionTasks.Find(x => x.type == collectionType).taskUI.GetComponent<UICollectionPanel>().AddNum(1,transform);
        Instantiate(Resources.Load<GameObject>("Effect/PickupTaskitem"));
        Destroy(gameObject);

    }


}
