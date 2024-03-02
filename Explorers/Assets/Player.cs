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

    public GameObject Rope;

    public GameObject Solver;

    public bool hasRope = true;

    private Vector3 _originalPos;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _batteryTransform = GameObject.Find("Battery").transform;
        _originalPos = transform.position;
        Debug.Log(_originalPos);
    }

    // Update is called once per frame
    void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");

        _movement.y = Input.GetAxisRaw("Vertical");


        CheckDistanceToBattery();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag=="BatteryTrigger")
        {
            if(!hasRope && Input.GetKeyDown(KeyCode.E))
            {
                hasRope = true;
                GameObject newRopeHanger = Instantiate(Resources.Load<GameObject>("Hanger"), transform.position, Quaternion.identity);
                newRopeHanger.transform.SetParent(Solver.transform);
                
                Rope = newRopeHanger;
                GameObject rope = Rope.transform.GetChild(0).gameObject;
                ObiParticleAttachment[] attachment = rope.GetComponents<ObiParticleAttachment>();
                attachment[0].target = transform;
                attachment[1].target = _batteryTransform;
                ObiRope obiRope = rope.GetComponent<ObiRope>();
            }
        }
    }
    private void CheckDistanceToBattery()
    {
        if(Vector3.Distance(_batteryTransform.position,transform.position)>DistanceThreshold)
        {
            Destroy(Rope,2f);
            hasRope = false;
        }
    }
    private void FixedUpdate()
    {
       // _rb.velocity = new Vector3(_movement.x * Speed, _movement.y*Speed, 0);
        _rb.transform.Translate(new Vector3(_movement.x, _movement.y,0 ).normalized * Time.deltaTime * Speed, Space.World);
    }
}
