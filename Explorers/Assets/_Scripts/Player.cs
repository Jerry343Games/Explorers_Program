using Obi;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed;

    private Vector3 _movement;

    private Rigidbody _rb;

    private Transform _batteryTransform;

    public float DistanceThreshold = 10;

    public GameObject RopeHanger;

    private ObiRope _obiRope;

    public GameObject Solver;

    public bool HasRope = true;

    public float OrigianalDistanceToBattery = 4.1f;//��ʼ���صľ���
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _obiRope = RopeHanger.transform.GetChild(0).GetComponent<ObiRope>();
    }
    void Start()
    {
        _batteryTransform = GameObject.Find("Battery").transform;
    }

    // Update is called once per frame
    void Update()
    {
        /*_movement.x = 0;*/
        _movement.x = Input.GetAxisRaw("Horizontal");

        /*_movement.y = 0;*/
        _movement.y = Input.GetAxisRaw("Vertical");


        CheckDistanceToBattery();
        DynamicChangeLengthOfRope();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag=="BatteryTrigger")
        {
            if(!HasRope && Input.GetKeyDown(KeyCode.E))
            {
                HasRope = true;
                //���㵱ǰ����Ӧ����ת�ĽǶ�
                float rotationZ = Vector3.Angle((transform.position - _batteryTransform.position).normalized, Vector3.right) * (transform.position.y < _batteryTransform.position.y ? -1 : 1);
                Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
                //��������Ԥ����
                GameObject newRopeHanger = Instantiate(Resources.Load<GameObject>("Hanger"), (transform.position + _batteryTransform.position) / 2, rotation);
                //���ݱ�׼�����ӳ��� �ı䵱ǰ��scale
                newRopeHanger.transform.localScale = new Vector3(Vector3.Distance(transform.position, _batteryTransform.position) / 4.2f, 1, 1);
                //���ø�������ʵ�����ӹ���
                newRopeHanger.transform.SetParent(Solver.transform);
                RopeHanger = newRopeHanger;
                GameObject rope = RopeHanger.transform.GetChild(0).gameObject;
                _obiRope = rope.GetComponent<ObiRope>();
                ObiParticleAttachment[] attachment = rope.GetComponents<ObiParticleAttachment>();
                //�����������ߵ�ǣ������
                attachment[0].target = _batteryTransform;
                attachment[1].target = transform;
            }
        }
    }

    /// <summary>
    /// ������صľ���
    /// </summary>
    private void CheckDistanceToBattery()
    {
        if(Vector3.Distance(_batteryTransform.position,transform.position)>DistanceThreshold)
        {
            Destroy(RopeHanger,2f);
            HasRope = false;
        }
    }

    /// <summary>
    /// ʵʱ��̬�������ӳ���
    /// </summary>
    private void DynamicChangeLengthOfRope()
    {
        if (_obiRope == null || gameObject.name =="Battery") return;
        _obiRope.stretchingScale = Vector3.Distance(transform.position, _batteryTransform.position) / 4.2f;
    }
    private void FixedUpdate()
    {
       // _rb.velocity = new Vector3(_movement.x * Speed, _movement.y*Speed, 0);
        _rb.transform.Translate(new Vector3(_movement.x, _movement.y,0 ).normalized * Time.deltaTime * Speed, Space.World);
    }
}
