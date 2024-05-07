using DG.Tweening;
using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

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

    public Image mask;

    public Image loseImg;

    public Image winImg;

    public RectTransform[] winCGPoints;

    public bool hasGameOver;

    public bool isPaused;
    
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
            hanger.transform.localScale = new Vector3(0.05f, 1, 1);
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

        //GameOver(true);

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
        hasGameOver = true;
        //时间停滞
        Time.timeScale = 0;

        MusicManager.Instance.musicAudio.Pause();

        StartCoroutine(GameOverAction(isWin));
        //UI显示

    }
    IEnumerator GameOverAction(bool isWin)
    {
        if(isWin)
        {
            Sequence s = DOTween.Sequence();
            s.SetUpdate(UpdateType.Normal, true);
            s.Append(mask.DOFade(1, 2f));
            winImg.transform.GetChild(0).gameObject.SetActive(true);
            s.Append(winImg.transform.GetChild(0).GetComponent<RectTransform>().DOAnchorPos(winCGPoints[0].anchoredPosition, 0.5f).OnStart(() =>
            {
                MusicManager.Instance.PlaySound("漫画滑入");

            }).OnComplete(() =>
            {
                MusicManager.Instance.PlaySound("机器启动");

            }));
            s.AppendInterval(1f);
            winImg.transform.GetChild(1).gameObject.SetActive(true);
            s.Append(winImg.transform.GetChild(1).GetComponent<RectTransform>().DOAnchorPos(winCGPoints[1].anchoredPosition, 0.5f).OnStart(() =>
            {
                MusicManager.Instance.PlaySound("漫画滑入");

            }).OnComplete(() =>
            {
                MusicManager.Instance.PlaySound("屏幕亮起");

            }));
            s.AppendInterval(0.5f);
            winImg.transform.GetChild(2).gameObject.SetActive(true);
            s.Append(winImg.transform.GetChild(2).GetComponent<RectTransform>().DOAnchorPos(winCGPoints[2].anchoredPosition, 0.5f).OnStart(() =>
            {
                MusicManager.Instance.PlaySound("漫画滑入");

            }));
            s.AppendInterval(2f);
            s.Append(winImg.DOFade(0, 0.5f));
            s.Append(winImg.transform.GetChild(0).GetComponent<Image>().DOFade(0, 0.5f));
            s.Append(winImg.transform.GetChild(1).GetComponent<Image>().DOFade(0, 0.5f));
            s.Append(winImg.transform.GetChild(2).GetComponent<Image>().DOFade(0, 0.5f));
            s.Append(winImg.DOFade(1, 3f));
            s.Append(winImg.DOFade(0, 3f).OnComplete(() =>
            {
                winImg.transform.GetChild(6).gameObject.SetActive(true);
                MusicManager.Instance.PlayBackMusic("Select");
            }));
            s.Append(winImg.transform.GetChild(6).GetComponent<RectTransform>().DOAnchorPos(
                new Vector2(winImg.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition.x, -winImg.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition.y),50f));
            s.AppendInterval(2f).OnComplete(() =>
            {
                Time.timeScale = 1;
                UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
            });


        }
        else
        {
            Sequence s = DOTween.Sequence();
            s.SetUpdate(UpdateType.Normal, true);

            s.Append(mask.DOFade(1, 1f));
            s.Append(loseImg.DOFade(1, 1f));
            s.Append(loseImg.transform.GetChild(0).GetComponent<Image>().DOFade(1, 1f));
            s.AppendInterval(5f).OnComplete(() =>
            {
                Time.timeScale = 1;
                UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
            });
            //重开
            

        }
        foreach (var player in PlayerManager.Instance.players)
        {
            Destroy(player.gameObject);
        }
        PlayerManager.Instance.players.Clear();
        PlayerManager.Instance.gamePlayers.Clear();
        PlayerManager.Instance.resNum = 0;

        yield return null;  
    }

    public void StopGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
            GameObject panel= Instantiate(Resources.Load<GameObject>("UI/PausePanel"), GameObject.FindWithTag("Canvas").transform);
            
            //导航锁定
            
            foreach (var player in PlayerManager.Instance.players)
            {
                player.GetComponent<PlayerInputSetting>().SwitchToUISchemeAndSelect(panel.GetComponent<UIPausePanel>().continueBtn.gameObject);
                //player.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(panel.GetComponent<UIPausePanel>().continueBtn.gameObject);
            }
        }
        
    }

    public void ShowLearnPanel()
    {
        GameObject panel = Instantiate(Resources.Load<GameObject>("UI/LearningPanel"));
        GameObject firstSelected = panel.GetComponent<UILearningPanel>().learnGroup[0].featureBtn.gameObject;
        foreach (var player in PlayerManager.Instance.players)
        {
            player.GetComponent<PlayerInputSetting>().SwitchToUISchemeAndSelect(firstSelected);
        }
    }

    public void ContinueGame()
    {
        foreach (var player in PlayerManager.Instance.players)
        {
            player.GetComponent<PlayerInputSetting>().SwitchToPlayerScheme();
        }
        isPaused = false;
        Time.timeScale = 1;
    }

    public void BackToMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        foreach (var player in PlayerManager.Instance.players)
        {
            Destroy(player.gameObject);
        }
        PlayerManager.Instance.players.Clear();
        PlayerManager.Instance.gamePlayers.Clear();
        PlayerManager.Instance.resNum = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }
}
