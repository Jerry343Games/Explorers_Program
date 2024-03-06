using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public ItemType itemType;//物品类型
    public  virtual void Apply(GameObject user) { }

}
