using Obi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SceneManager : Singleton<SceneManager>
{
    public int maxPlayer=2;
    public bool isMaxPlayer;
    private GameObject[] _players;
    [HideInInspector]
    public Transform BatteryTransform;//电池坐标（本地坐标）
    [HideInInspector]
    public GameObject Slover;//使得绳子生效的父物体



    private void OnEnable()
    {
        EventCenter.GameStartedEvent += GameInit;
    }

    private void OnDisable()
    {
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
        if (_players.Length==maxPlayer)
        {
            isMaxPlayer = true;
            //获得电池玩家的位置 因为绳子都需要连到他身上
            BatteryTransform = _players.ToList().Find(x =>x.transform.GetChild(0).gameObject.activeInHierarchy).transform.GetChild(0);
            //暂时满人了直接开始 后面可能添加倒计时之类的
            EventCenter.CallGameStartedEvent();
            Debug.Log("MaxPlayer!:");
        }
    }

    private void GameInit()
    {

        foreach (var player in _players)
        {
            //电池不连电池
            if (player == BatteryTransform.parent.gameObject) continue;
            PlayerController controller = null;
            for (int i=0;i< player.transform.childCount;i++)
            {
                if(player.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    controller = player.transform.GetChild(i).GetComponent<PlayerController>();
                }
            }
            float rotationZ = Vector3.Angle((player.transform.GetChild(controller.myIndex).position - BatteryTransform.position).normalized, Vector3.right) * (player.transform.GetChild(controller.myIndex).position.y < BatteryTransform.position.y ? -1 : 1);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
            GameObject hanger = Instantiate(Resources.Load<GameObject>("Hanger"), (player.transform.GetChild(controller.myIndex).position + BatteryTransform.position) / 2 , rotation);
            //暂定0.2f 以后希望玩家一开始都不能动全到齐之后生成绳子
            hanger.transform.localScale = new Vector3(0.2f, 1, 1);
            //设置父物体以实现绳子功能
            hanger.transform.SetParent(Slover.transform);
            GameObject rope = hanger.transform.GetChild(0).gameObject;
            ObiParticleAttachment[] attachment = rope.GetComponents<ObiParticleAttachment>();
            ObiRope obiRope = rope.GetComponent<ObiRope>();
            //设置绳子两边的牵引对象
            attachment[0].target = BatteryTransform;
            attachment[1].target = player.transform.GetChild(controller.myIndex);
            controller.SetRope(obiRope);
        }
    }
}
