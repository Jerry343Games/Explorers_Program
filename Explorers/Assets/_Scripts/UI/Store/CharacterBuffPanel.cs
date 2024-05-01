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

    public List<UpgradeBuff> availableBuffs; // 所有可能的Buff
    private List<UpgradeBuff> _displayedBuffs = new List<UpgradeBuff>(); // 当前展示的Buff

    public GameObject[] buffSlots;//展示位
    
    
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
        
        RefreshBuffs();
    }
    
    /// <summary>
    ///随机刷新Buff缓存
    /// </summary>
    public void RefreshBuffs() 
    {
        _displayedBuffs.Clear();
        List<UpgradeBuff> tempBuffs = new List<UpgradeBuff>(availableBuffs);
        for (int i = 0; i < 3; i++) {
            int randIndex = Random.Range(0, tempBuffs.Count);
            _displayedBuffs.Add(tempBuffs[randIndex]);
            tempBuffs.RemoveAt(randIndex);
        }
        ShowSlotsOnUI();
    }

    /// <summary>
    /// 从缓存读取并分发给子级UI组件
    /// </summary>
    public void ShowSlotsOnUI()
    {
        for (int i = 0; i < buffSlots.Length; i++)
        {
            buffSlots[i].GetComponent<BuffItem>().myBuff = _displayedBuffs[i];
            buffSlots[i].GetComponent<BuffItem>().EnableRefresh();
        }
    }
}
