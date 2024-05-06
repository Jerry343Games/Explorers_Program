using Obi;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class CollectionTask//�ռ�������
{
    public CollectionType type;
    public int amount;
    public GameObject taskUI;
    public bool hasFinshed;
}
[Serializable]
public class ResTask//�ɼ���Դ����
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
    public Transform BatteryTransform;//������꣨�������꣩
    [HideInInspector]
    public GameObject Slover;//ʹ��������Ч�ĸ�����
    public Transform bornTransform;

    public bool hasMainBattary;
    public List<CollectionTask> collectionTasks = new List<CollectionTask>();//�ɼ����ռ������б�

    public List<ResTask> resTasks = new List<ResTask>();//��Դ�ռ������б�

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

        //�����Թؿ�������Ϸ�ã�������Ǵ�ѡ���ɫ�ؽ���Ҳ�ܿ���Ϸ
        if (_players.Length == maxPlayer && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "SelectScene")
        {
            isMaxPlayer = true;
            //��õ����ҵ�λ�� ��Ϊ���Ӷ���Ҫ����������
            BatteryTransform = _players.ToList().Find(x => x.transform.GetChild(0).gameObject.activeInHierarchy).transform.GetChild(0);
            //��ʱ������ֱ�ӿ�ʼ ���������ӵ���ʱ֮���
            EventCenter.CallGameStartedEvent();
        }

    }

    public void FindBattery()
    {
        //��õ����ҵ�λ�� ��Ϊ���Ӷ���Ҫ����������
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
            //�����ѡ����
            controller.feature = controller.playerInputSetting.feature;
            controller.SetFeatureCD();

            controller.transform.position = bornTransform.position;


            //��ɫUI�������
            GameObject playerHealthPanel = Instantiate(Resources.Load<GameObject>("UI/" + controller.gameObject.name + "Panel"));

            playerHealthPanel.transform.SetParent(healthPanel.transform, false);

            playerHealthPanel.GetComponent<RectTransform>().anchoredPosition = panelPoints[uiIndex];
            uiIndex++;
            //��ز������
            if (player == BatteryTransform.parent.gameObject)
            {
                EnemyManager.Instance.battery = controller.gameObject;
                continue;
            }
            float rotationZ = Vector3.Angle((player.transform.GetChild(index).position - BatteryTransform.position).normalized, Vector3.right) *
                (player.transform.GetChild(index).position.y < BatteryTransform.position.y ? -1 : 1);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
            GameObject hanger = Instantiate(Resources.Load<GameObject>("Hanger"), (player.transform.GetChild(index).position + BatteryTransform.position) / 2 , rotation);
            //�ݶ�0.2f �Ժ�ϣ�����һ��ʼ�����ܶ�ȫ����֮����������
            hanger.transform.localScale = new Vector3(0.15f, 1, 1);
            //���ø�������ʵ�����ӹ���
            hanger.transform.SetParent(Slover.transform);
            GameObject rope = hanger.transform.GetChild(0).gameObject;
            ObiParticleAttachment[] attachment = rope.GetComponents<ObiParticleAttachment>();
            ObiRope obiRope = rope.GetComponent<ObiRope>();
            //�����������ߵ�ǣ������
            attachment[0].target = BatteryTransform;
            attachment[1].target = player.transform.GetChild(index);
            controller.SetRope(obiRope);

        }

        //������һ��
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
        //����ӳ�����UI��Player
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
        //ʱ��ͣ��
        Time.timeScale = 0;
        //UI��ʾ

    }
}
