using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

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
    private HashSet<BuffType> _allowedTypes;//当前允许的类型

    public GameObject[] buffSlots;//展示位
    public RectTransform buffList;//整个展示列表
    public RectTransform listMask;//整个展示列表的遮罩

    public Button refreshBtn;
    public Button confirmBtn;

    private Vector2 _listEndValue=new Vector2(-0.716601f, 2.109364f);
    private Vector2 _listStartValue=new Vector2(-0.716601f,-226.7094f);

    private bool _isFirst;

    public static event Action OnRefreshBtnClick_Can;
    public static event Action OnRefreshBtnClick_Cant;

    public event Action OnConfirmClick; 
    private void OnEnable()
    {
        _isFirst = true;
        _allowedTypes=new HashSet<BuffType>();
        refreshBtn.onClick.AddListener(ClickRefreshBtn);
        confirmBtn.onClick.AddListener(ClickConfirmBtn);

        buffList.anchoredPosition = new Vector2(0, -226);

        for (int i = 0; i < buffSlots.Length; i++)
        {
            buffSlots[i].GetComponent<BuffItem>().chooseBuff += OnClickBuffBtn;


        }
    }

    private void OnDestroy()
    {
        refreshBtn.onClick.RemoveListener(ClickRefreshBtn);
        confirmBtn.onClick.RemoveListener(ClickConfirmBtn);
        
        //对全部子级UI组件设置监听
        for (int i = 0; i < buffSlots.Length; i++)
        {
            buffSlots[i].GetComponent<BuffItem>().chooseBuff -= OnClickBuffBtn;
        }
    }

    public void SetPlayerToItems()
    {

        for (int i = 0; i < buffSlots.Length; i++)
        {
            buffSlots[i].GetComponent<BuffItem>().player = player.GetComponentInChildren<PlayerController>() ;
        }
    }

    public void Refresh()
    {
        myPlayerInfo = player.GetComponentInChildren<PlayerController>().myPlayerInfo;

        switch (myPlayerInfo.playerType)
        {
            case PlayerType.BatteryCarrier:
                playerImg.sprite = Resources.Load<Sprite>("UI/Image/Battery - Copy");
                _allowedTypes=new HashSet<BuffType> { BuffType.BatteryCarrier, BuffType.General };
                RefreshBuffs(_allowedTypes);
                break;
            case PlayerType.Shooter:
                playerImg.sprite = Resources.Load<Sprite>("UI/Image/Shooter");
                _allowedTypes=new HashSet<BuffType> { BuffType.Shooter, BuffType.General,BuffType.Explorers };
                break;
            case PlayerType.Fighter:
                playerImg.sprite = Resources.Load<Sprite>("UI/Image/Fighter");
                _allowedTypes=new HashSet<BuffType> { BuffType.Fighter, BuffType.General,BuffType.Explorers };
                break;
            case PlayerType.Healer:
                playerImg.sprite = Resources.Load<Sprite>("UI/Image/Healer");
                _allowedTypes=new HashSet<BuffType> { BuffType.Healer, BuffType.General,BuffType.Explorers };
                break;
        }
        //操作分配到的玩家事件系统和输入系统
        MultiplayerEventSystem multiEven = player.GetComponent<MultiplayerEventSystem>();
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
        multiEven.firstSelectedGameObject = firstbuff;
        multiEven.playerRoot = gameObject;
        multiEven.SetSelectedGameObject(firstbuff);
        
        RefreshBuffs(_allowedTypes);
        _isFirst = false;
    }
    
    /// <summary>
    /// 根据允许的buffType随机分配
    /// </summary>
    /// <param name="allowedTypes"></param>
    public void RefreshBuffs(HashSet<BuffType> allowedTypes) 
    {
        _displayedBuffs.Clear();
        List<UpgradeBuff> tempBuffs = new List<UpgradeBuff>(availableBuffs);
        int buffsAdded = 0;

        while (buffsAdded < 3 && tempBuffs.Count > 0) {
            int randIndex = Random.Range(0, tempBuffs.Count);
            UpgradeBuff selectedBuff = tempBuffs[randIndex];
            if (allowedTypes.Contains(selectedBuff.buffType)) {
                _displayedBuffs.Add(selectedBuff);
                buffsAdded++;
            }
            tempBuffs.RemoveAt(randIndex);
        }

        if (_isFirst)
        {
            buffList.DOAnchorPos(_listEndValue, 0.5f);
            ShowSlotsOnUI();
        }
        else
        {
            Sequence q = DOTween.Sequence();
            q.Append(buffList.DOAnchorPos(_listStartValue, 0.5f).OnComplete(ShowSlotsOnUI));
            q.AppendInterval(0.2f);
            q.Append(buffList.DOAnchorPos(_listEndValue, 0.5f));
        }
    }

    /// <summary>
    /// 从写入buffSlots的读取并分发给子级UI组件
    /// </summary>
    public void ShowSlotsOnUI()
    {
        for (int i = 0; i < buffSlots.Length; i++)
        {
            buffSlots[i].GetComponent<BuffItem>().myBuff = _displayedBuffs[i];
            buffSlots[i].GetComponent<BuffItem>().EnableRefresh();
            buffSlots[i].GetComponent<BuffItem>().RefreshNextRound();
        }
    }

    private void ClickRefreshBtn()
    {
        if (PlayerManager.Instance.resNum>20)
        {
            RefreshBuffs(_allowedTypes);
            PlayerManager.Instance.ChangeResData(-20);
            OnRefreshBtnClick_Can?.Invoke();
        }
        else
        {
            OnRefreshBtnClick_Cant?.Invoke();
        }
    }

    /// <summary>
    /// 点击确认事件
    /// </summary>
    private void ClickConfirmBtn()
    {
        listMask.DOAnchorPos(Vector2.zero, 0.5f);
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        
        OnConfirmClick?.Invoke();

        
    }

    /// <summary>
    /// 监听子级UIBuff选择的事件，执行锁定操作
    /// </summary>
    private void OnClickBuffBtn()
    {
        foreach (GameObject buffslot in buffSlots)
        {
            BuffItem buffItem= buffslot.GetComponent<BuffItem>();
            buffItem.isEndOneRound = true;
            if (buffItem.isSelected)
            {
                buffItem.ShowChooseMask();
            }
            else
            {
                buffItem.ShowUnChooseMask();
            }
        }
    }

    private void OnConfirm()
    {
        listMask.DOAnchorPos(Vector2.zero, 0.5f);
        OnConfirmClick?.Invoke();
    }
    
}
