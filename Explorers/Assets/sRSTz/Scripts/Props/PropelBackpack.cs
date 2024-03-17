using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropelBackpack : Item
{
    public float activeTime = 2f;
    private float activeTimer = 0;
    public bool isUsing = false;
    public float targetSpeed = 15f;
    public SphereCollider sphereCollider;
    public GameObject sprite;
    public GameObject attackArea;
    public bool isPicked =false;
    private float userSpeed;
    public float attackForce = 5f;
    public GameObject user;
    public int damage = 3;
    public override void Apply(GameObject user)
    {
        if (isPicked) return;
        
        isPicked = true;
        sprite.SetActive(false);
        this.user = user;
        userSpeed = user.GetComponent<PlayerController>().speed;
        sphereCollider.enabled = false;
        user.GetComponent<PlayerController>().item = this;
        Use(user);
    }
    private void Awake()
    {
        //GravityChange();
        sprite = transform.GetChild(0).gameObject;
        sphereCollider = GetComponent<SphereCollider>();
        

    }
    public override void Use(GameObject user)
    {
        user.layer = LayerMask.NameToLayer("PropelBackpackUser");
        attackArea.SetActive(true);
        user.GetComponent<PlayerController>().item = null;
        isUsing = true;
        user.GetComponent<PlayerController>().speed = targetSpeed;
        // Á¢¼´Ö´ÐÐÅö×²¼ì²â
        Physics.SyncTransforms();
        Physics.Simulate(Time.fixedDeltaTime);

    }
    private void Update()
    {
        if (isUsing)
        {
            activeTimer += Time.deltaTime;
            transform.position = user.transform.position;


            if (activeTimer >= activeTime)
            {
                Exit();
            }
        }  
    }
    public void Exit()
    {
        isUsing = false;
        activeTimer = 0;
        user.GetComponent<PlayerController>().speed = userSpeed;
        user.layer = LayerMask.NameToLayer("Player");
        user = null;
        Destroy(gameObject);
    }


}
