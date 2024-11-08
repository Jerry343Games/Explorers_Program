using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleManager:MonoBehaviour
{
    public GameObject bubblePrefab; // 气泡的预制体

    private GameObject currentBubble;

    public Sprite F;
    public Sprite E;
    public Sprite A;
    public Sprite Y;
    public delegate void BubbleCreateHandler(BubbleInfo info);
    public event BubbleCreateHandler OnBubbleCreate;
    private void Awake()
    {
        OnBubbleCreate += CreateBubble;
    }

    public void CreateBubble(BubbleInfo info)
    {
        // 实例化气泡
        // 销毁已有的气泡
        if (currentBubble != null)
        {
            Destroy(currentBubble);
        }
        currentBubble = Instantiate(bubblePrefab);
        UIBubbleItem bubbleItem = currentBubble.GetComponent<UIBubbleItem>();
        
        if (bubbleItem != null)
        {
            if (info.myControlType=="Keyboard")
            {
                //键盘
                switch (info.Type)
                {
                    case BubbleType.Interact:
                        bubbleItem.myImage.sprite = E;
                        break;
                    case BubbleType.Reconnect:
                        bubbleItem.myImage.sprite = F;
                        break;
                }
            }
            else if (info.myControlType=="Controller")
            {
                switch (info.Type)
                {
                    case BubbleType.Interact:
                        bubbleItem.myImage.sprite = Y;
                        break;
                    case BubbleType.Reconnect:
                        bubbleItem.myImage.sprite = A;
                        break;
                }
            }
            else
            {
                switch (info.Type)
                {
                    case BubbleType.Interact:
                        bubbleItem.myImage.sprite = E;
                        break;
                    case BubbleType.Reconnect:
                        bubbleItem.myImage.sprite = F;
                        break;
                }
            }
            
            // 设置气泡信息
            bubbleItem.gameObject1 = info.Obj1;
            bubbleItem.gameObject2 = info.Obj2;
            bubbleItem.instruction = info.Content;
            bubbleItem.Init(); // 初始化气泡
        }
    }

    public void DestroyBubble()
    {
        if (currentBubble)
        {
            currentBubble.GetComponent<UIBubbleItem>().DestoryBubble();
        }
    }
}
