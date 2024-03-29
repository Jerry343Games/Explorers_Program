using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;



[Serializable]
public struct CollectionTask//收集物任务
{
    public CollectionType type;
    public int amount;
    public GameObject taskUI;
    public bool hasFinshed;
}
[Serializable]
public struct ResTask//采集资源任务
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
    
    public List<GameObject> players = new List<GameObject>();//玩家列表

    private void OnEnable()
    {
        EventCenter.GameStartedEvent += ConnectRopeToBattary;
        EventCenter.GameStartedEvent += GameInit;
    }

    private void OnDisable()
    {
        EventCenter.GameStartedEvent -= ConnectRopeToBattary;
        EventCenter.GameStartedEvent -= GameInit;
    }
    void Start()
    {
        Slover = GameObject.Find("Solver");
    }

    void Update()
    {
        if (!isMaxPlayer)
        {
            CountPlayer();
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

    private void ConnectRopeToBattary()
    {
        //获得电池玩家的位置 因为绳子都需要连到他身上
        BatteryTransform = _players.ToList().Find(x =>x.transform.GetChild(0).gameObject.activeInHierarchy).transform.GetChild(0);

    }

    private void GameInit()
    {

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
            //电池不连电池
            if (player == BatteryTransform.parent.gameObject) continue;
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
    }
    
    /// <summary>
    /// 注册玩家
    /// </summary>
    /// <param name="player"></param>
    public void RegisterPlayer(GameObject player)
    {
        Debug.Log("Register Player");
        players.Add(player);
    }
    
    /// <summary>
    /// 获得最后一个注册的玩家
    /// </summary>
    /// <returns></returns>
    public GameObject GetLatestPlayer()
    {
        if (players.Count > 0)
        {
            Debug.Log("GetLastPlayer");
            return players[players.Count - 1];
        }

        return null;
    }

    /// <summary>
    /// 销毁所有玩家
    /// </summary>
    public void DestoryAllPlayers()
    {
        foreach(GameObject player in players)
        {
            Destroy(player);
        }
        players.Clear();
    }
}
