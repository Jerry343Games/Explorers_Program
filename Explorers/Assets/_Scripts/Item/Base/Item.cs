using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public ItemType itemType;//物品类型
    public  virtual void Apply(GameObject user) { }
    /// <summary>
    /// 只有拾取后能再使用的道具实现这个方法
    /// </summary>
    /// <param name="user"></param>
    public virtual void Use(GameObject user) { }


}
