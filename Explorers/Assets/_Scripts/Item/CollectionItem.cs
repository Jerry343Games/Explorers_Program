using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionItem : Item
{
    public CollectionType collectionType;

    public override void Apply(GameObject user)
    {
        //增加收集物数量
        Destroy(gameObject);

    }


}
