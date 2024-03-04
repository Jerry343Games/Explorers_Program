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
    public Transform BatteryTransform;//������꣨�������꣩
    [HideInInspector]
    public GameObject Slover;//ʹ��������Ч�ĸ�����



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
            //��õ����ҵ�λ�� ��Ϊ���Ӷ���Ҫ����������
            BatteryTransform = _players.ToList().Find(x =>x.transform.GetChild(0).gameObject.activeInHierarchy).transform.GetChild(0);
            //��ʱ������ֱ�ӿ�ʼ ���������ӵ���ʱ֮���
            EventCenter.CallGameStartedEvent();
            Debug.Log("MaxPlayer!:");
        }
    }

    private void GameInit()
    {

        foreach (var player in _players)
        {
            //��ز������
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
            //�ݶ�0.2f �Ժ�ϣ�����һ��ʼ�����ܶ�ȫ����֮����������
            hanger.transform.localScale = new Vector3(0.2f, 1, 1);
            //���ø�������ʵ�����ӹ���
            hanger.transform.SetParent(Slover.transform);
            GameObject rope = hanger.transform.GetChild(0).gameObject;
            ObiParticleAttachment[] attachment = rope.GetComponents<ObiParticleAttachment>();
            ObiRope obiRope = rope.GetComponent<ObiRope>();
            //�����������ߵ�ǣ������
            attachment[0].target = BatteryTransform;
            attachment[1].target = player.transform.GetChild(controller.myIndex);
            controller.SetRope(obiRope);
        }
    }
}
