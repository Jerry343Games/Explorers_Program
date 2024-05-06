using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleInfo
{
    public string myControlType;
    public BubbleType Type { get; set; }//气泡类型
    public GameObject Obj1 { get; set; }
    public GameObject Obj2 { get; set; }
    public string Content { get; set; }//文本
}

public class UIBubblePanel : MonoBehaviour
{
    
    // 创建静态实例变量
    public static UIBubblePanel Instance { get; private set; }
    
    public delegate void BubbleCreateHandler(BubbleInfo info);
    public event BubbleCreateHandler OnBubbleCreate;

    public GameObject interectBubbleBuffer;
    public GameObject reconnectCableBuffer;

    private UIBubbleItem _bubbleItem;
    public void CreateBubble(BubbleInfo info)
    {
        OnBubbleCreate?.Invoke(info);
    }
    
    private void Awake()
    {
        // 检查实例是否已存在
        if (Instance == null)
        {
            // 如果实例不存在，则设置当前对象为实例
            Instance = this;
        }
        else if (Instance != this)
        {
            // 如果当前实例不是已存在的实例，则销毁它
            Destroy(gameObject);
        }
        
        OnBubbleCreate += HandleBubbleCreation;
    }
    
    private void OnDestroy()
    {
        // 防止内存泄漏
        OnBubbleCreate -= HandleBubbleCreation;
    }
    
    private void HandleBubbleCreation(BubbleInfo info)
    {
        // 在这里使用info的信息来创建和显示气泡
        Vector3 pos = Camera.main.WorldToScreenPoint(GetCenterPoint(info.Obj1.transform.position,info.Obj2.transform.position));
        
        switch (info.Type)
        {
            // case BubbleType.ResourceCollectionBubble:
            //     interectBubbleBuffer = Instantiate(Resources.Load<GameObject>("UI/InteractBubble"),pos,quaternion.identity,transform);
            //     _bubbleItem = interectBubbleBuffer.GetComponent<UIBubbleItem>();
            //     break;
            // case BubbleType.ReconnectCableBubble:
            //     reconnectCableBuffer = Instantiate(Resources.Load<GameObject>("UI/ReconnectBubble"),pos,quaternion.identity,transform);
            //     _bubbleItem = reconnectCableBuffer.GetComponent<UIBubbleItem>();
            //     break;
            //case BubbleType.ChestOpenBubble:
            //    interectBubbleBuffer = Instantiate(Resources.Load<GameObject>("UI/InteractBubble"), pos, quaternion.identity, transform);
            //    _bubbleItem = interectBubbleBuffer.GetComponent<UIBubbleItem>();
            //    break;
            default:
                break;
        }

        if (_bubbleItem)
        {
            _bubbleItem.gameObject1 = info.Obj1;
            _bubbleItem.gameObject2 = info.Obj2;
            _bubbleItem.instruction = info.Content;
        }
    }
    
    public Vector3 GetCenterPoint(Vector3 position1, Vector3 position2)
    {
        return (position1 + position2) / 2.0f;
    }
}
