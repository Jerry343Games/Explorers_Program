using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleInfo
{
    public string myControlType;
    public BubbleType Type { get; set; }//��������
    public GameObject Obj1 { get; set; }
    public GameObject Obj2 { get; set; }
    public string Content { get; set; }//�ı�
}

public class UIBubblePanel : MonoBehaviour
{
    
    // ������̬ʵ������
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
        // ���ʵ���Ƿ��Ѵ���
        if (Instance == null)
        {
            // ���ʵ�������ڣ������õ�ǰ����Ϊʵ��
            Instance = this;
        }
        else if (Instance != this)
        {
            // �����ǰʵ�������Ѵ��ڵ�ʵ������������
            Destroy(gameObject);
        }
        
        OnBubbleCreate += HandleBubbleCreation;
    }
    
    private void OnDestroy()
    {
        // ��ֹ�ڴ�й©
        OnBubbleCreate -= HandleBubbleCreation;
    }
    
    private void HandleBubbleCreation(BubbleInfo info)
    {
        // ������ʹ��info����Ϣ����������ʾ����
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
