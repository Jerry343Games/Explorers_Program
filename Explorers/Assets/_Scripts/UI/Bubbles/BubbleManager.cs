using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : Singleton<SceneManager>
{
    public GameObject bubblePrefab; // 气泡的预制体

    public delegate void BubbleCreateHandler(BubbleInfo info);
    public event BubbleCreateHandler OnBubbleCreate;
    private void Awake()
    {
        OnBubbleCreate += CreateBubble;
    }

    private void CreateBubble(BubbleInfo info)
    {
        // 实例化气泡
        GameObject bubbleInstance = Instantiate(bubblePrefab);
        UIBubbleItem bubbleItem = bubbleInstance.GetComponent<UIBubbleItem>();
        
        if (bubbleItem != null)
        {
            // 设置气泡信息
            bubbleItem.gameObject1 = info.Obj1;
            bubbleItem.gameObject2 = info.Obj2;
            bubbleItem.instruction = info.Content;
            bubbleItem.Init(); // 初始化气泡
        }
    }

    private void OnDestroy()
    {
        OnBubbleCreate -= CreateBubble;
    }
}
