using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public ItemType itemType;//��Ʒ����
    public  virtual void Apply(GameObject user) { }
    /// <summary>
    /// ֻ��ʰȡ������ʹ�õĵ���ʵ���������
    /// </summary>
    /// <param name="user"></param>
    public virtual void Use(GameObject user) { }


}
