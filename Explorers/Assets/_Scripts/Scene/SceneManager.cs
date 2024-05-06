using Obi;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class CollectionTask//收集物任务
{
    public CollectionType type;
    public int amount;
    public GameObject taskUI;
    public bool hasFinshed;
}
[Serializable]
public class ResTask//采集资源任务
{
    public ResourceType type;
    public int amount;
    public GameObject taskUI;
    public bool hasFinshed;
}
public class SceneManager : Singleton<SceneManager>
{
    public int maxPlayer=2;
    public bool isMaxPlayer;
    private GameObject[] _players;
    //[HideInInspector]
    public Transform BatteryTransform;//电池坐标（本地坐标）
    [HideInInspector]
    public GameObject Slover;//使得绳子生效的父物体
    public Transform bornTransform;

    public bool hasMainBattary;
    public List<CollectionTask> collectionTasks = new List<CollectionTask>();//采集物收集任务列表

    public List<ResTask> resTasks = new List<ResTask>();//资源收集任务列表

    public int currentResNum = 0;

    public GameObject healthPanel;

    public List<Vector3> panelPoints = new List<Vector3>();

    public bool isSecondLevel;

    public bool matchVictoryCondition;

    public bool hasGameEnd;
    private void OnEnable()
    {
        //EventCenter.GameStartedEvent += FindBattery;
        EventCenter.GameStartedEvent += GameInit;
    }

    private void OnDisable()
    {
        //EventCenter.GameStartedEvent -= FindBattery;
        EventCenter.GameStartedEvent -= GameInit;
    }
    protected override void Awake()
    {
        base.Awake();
        maxPlayer = PlayerManager.Instance.players.Count;
        Slover = GameObject.Find("Solver");
        _players = GameObject.FindGameObjectsWithTag("BasePlayer");

        FindBattery();
    }

    private void Start()
    {
        bornTransform = GameObject.FindGameObjectWithTag("Start").transform;
        CameraTrace.instance.SetOriginalPos();
        foreach (var collection in FindObjectsOfType<CollectionItem>())
        {
            switch(collection.collectionType)
            {
                case CollectionType.PowerFurnaceParts:
                    collectionTasks[0].amount++;
                    break;
                case CollectionType.Antenna:
                    collectionTasks[1].amount++;
                    break;
                case CollectionType.StorageAreaShell:
                    collectionTasks[2].amount++;
                    break;
                default:
                    break;
            }
        }
        foreach(var task in collectionTasks)
        {
            task.taskUI.GetComponent<UICollectionPanel>().Init();
        }



    }

    void Update()
    {
        if (!isMaxPlayer)
        {
            CountPlayer();
        }
        if(!hasGameEnd)
        {
            if(CheckGameWinCondition())
            {
                matchVictoryCondition = true;
                //GameOver(true);
            }
        }

    }
    private void CountPlayer()
    {
        _players = GameObject.FindGameObjectsWithTag("BasePlayer");

        //给测试关卡开启游戏用，如果不是从选择角色关进入也能开游戏
        if (_players.Length == maxPlayer && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "SelectScene")
        {
            isMaxPlayer = true;
            //获得电池玩家的位置 因为绳子都需要连到他身上
            BatteryTransform = _players.ToList().Find(x => x.transform.GetChild(0).gameObject.activeInHierarchy).transform.GetChild(0);
            //暂时满人了直接开始 后面可能添加倒计时之类的
            EventCenter.CallGameStartedEvent();
        }

    }

    public void FindBattery()
    {
        //获得电池玩家的位置 因为绳子都需要连到他身上
        BatteryTransform = _players.ToList().Find(x =>x.transform.GetChild(0).gameObject.activeInHierarchy).transform.GetChild(0);

    }

    public void GameInit()
    {
        int uiIndex = 0;
        foreach (var player in _players)
        {
            PlayerController controller = null;
            int index = 0;
            for (int i = 0; i < player.transform.childCount; i++)
            {
                if (player.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    controller = player.transform.GetChild(i).GetComponent<PlayerController>();
                    index = i;
                    break;

                }
            }
            //获得自选功能
            controller.feature = controller.playerInputSetting.feature;
            controller.SetFeatureCD();

            controller.transform.position = bornTransform.position;


            //角色UI面板生成
            GameObject playerHealthPanel = Instantiate(Resources.Load<GameObject>("UI/" + controller.gameObject.name + "Panel"));

            playerHealthPanel.transform.SetParent(healthPanel.transform, false);

            playerHealthPanel.GetComponent<RectTransform>().anchoredPosition = panelPoints[uiIndex];
            uiIndex++;
            //电池不连电池
            if (player == BatteryTransform.parent.gameObject)
            {
                EnemyManager.Instance.battery = controller.gameObject;
                continue;
            }
            float rotationZ = Vector3.Angle((player.transform.GetChild(index).position - BatteryTransform.position).normalized, Vector3.right) *
                (player.transform.GetChild(index).position.y < BatteryTransform.position.y ? -1 : 1);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
            GameObject hanger = Instantiate(Resources.Load<GameObject>("Hanger"), (player.transform.GetChild(index).position + BatteryTransform.position) / 2 , rotation);
            //暂定0.2f 以后希望玩家一开始都不能动全到齐之后生成绳子
            hanger.transform.localScale = new Vector3(0.15f, 1, 1);
            //设置父物体以实现绳子功能
            hanger.transform.SetParent(Slover.transform);
            GameObject rope = hanger.transform.GetChild(0).gameObject;
            ObiParticleAttachment[] attachment = rope.GetComponents<ObiParticleAttachment>();
            ObiRope obiRope = rope.GetComponent<ObiRope>();
            //设置绳子两边的牵引对象
            attachment[0].target = BatteryTransform;
            attachment[1].target = player.transform.GetChild(index);
            controller.SetRope(obiRope);

        }

        //载入下一关
        if (!isSecondLevel)
        {
            MusicManager.Instance.PlayBackMusic("Level_1");
        }
        else
        {
            MusicManager.Instance.PlayBackMusic("Level_2");
        }

        Invoke("ChangeActionMap", 0.5f);
    }

    public void ChangeActionMap()
    {
        //更改映射表由UI到Player
        foreach (var player in PlayerManager.Instance.players)
        {
            player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        }
    }

    public bool CheckGameWinCondition()
    {
        foreach(var task in collectionTasks)
        {
            if(!task.hasFinshed)
            {
                return false;
            }    
        }
        foreach (var task in resTasks)
        {
            if (!task.hasFinshed)
            {
                return false;
            }
        }
        if(isSecondLevel)
        {
            if(GiantRockCrab.Instance)
            {
                if (GiantRockCrab.Instance.hasDead)
                {
                    hasGameEnd = true;
                    GameObject.FindGameObjectWithTag("Portal").GetComponent<SphereCollider>().enabled = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            hasGameEnd = true;
            GameObject.FindGameObjectWithTag("Portal").GetComponent<SphereCollider>().enabled = true;
            return true;
        }
    }
    
    public void GameOver(bool isWin)
    {
        //时间停滞
        Time.timeScale = 0;
        //UI显示

    }
}
