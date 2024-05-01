using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class CharacterBuffPanel : MonoBehaviour
{
    //玩家本体（父）,外部赋值
    public GameObject player;
    //玩家数据清单
    public PlayerInfo myPlayerInfo;
    //玩家头像UI
    public Image playerImg;
    //玩家需要第一个选择的物体
    public GameObject firstbuff;

    public void Refresh()
    {
        myPlayerInfo = player.GetComponentInChildren<PlayerController>().myPlayerInfo;
        
        switch (myPlayerInfo.playerType)
        {
            case PlayerType.BatteryCarrier:
                playerImg.sprite = Resources.Load<Sprite>("UI/Image/Battery - Copy");
                break;
            case PlayerType.Shooter:
                playerImg.sprite = Resources.Load<Sprite>("UI/Image/Shooter");
                break;
            case PlayerType.Fighter:
                playerImg.sprite = Resources.Load<Sprite>("UI/Image/Fighter");
                break;
            case PlayerType.Healer:
                playerImg.sprite = Resources.Load<Sprite>("UI/Image/Healer");
                break;
        }

        //操作分配到的玩家事件系统和输入系统
        MultiplayerEventSystem multiEven = player.GetComponent<MultiplayerEventSystem>();
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        multiEven.firstSelectedGameObject = firstbuff;
        multiEven.playerRoot = gameObject;
        multiEven.SetSelectedGameObject(firstbuff);
    }
}
