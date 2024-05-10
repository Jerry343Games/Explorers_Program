using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropelBackpack : PropItem
{

    public float activeTime = 2f;
    private float activeTimer = 0;
    public bool isUsing = false;
    //public float targetSpeed = 15f;
    public SphereCollider sphereCollider;
    public GameObject sprite;
    public GameObject attackArea;
    public bool isPicked =false;
    //private float userSpeed;
    public float attackForce = 5f;
    public GameObject user;
    public int damage = 3;
    public float dashForce = 10f;
    public float dashTime = 1.5f;
    public override void Apply(GameObject user)
    {
        if (isPicked) return;
        
        isPicked = true;
        sprite.SetActive(false);
        this.user = user;
        
        sphereCollider.enabled = false;
        user.GetComponent<PlayerController>().item = this;
        
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
        
        user.GetComponent<PlayerController>().item = null;
        isUsing = true;
        //user.GetComponent<PlayerController>().speed = targetSpeed;
        PlayerController player = user.GetComponent<PlayerController>();
        user.GetComponent<Rigidbody>().mass = 1.2f;
        player.Vertigo(player.gun.transform.forward * dashForce, ForceMode.VelocityChange, dashTime);
        attackArea.SetActive(true);
        

    }
    private void Update()
    {
        if (isUsing)
        {
            activeTimer += Time.deltaTime;
            transform.position = user.transform.position;
            user.GetComponent<PlayerController>()._dashParticleSystem.SetActive(true);

            if (activeTimer >= activeTime)
            {
                Exit();
            }
        }  
    }
    public void Exit()
    {
        isUsing = false;
        user.GetComponent<PlayerController>()._dashParticleSystem.SetActive(false);
        activeTimer = 0;
        user.GetComponent<Rigidbody>().mass = 1;
        //user.GetComponent<PlayerController>().speed = userSpeed;
        user.layer = LayerMask.NameToLayer("Player");
        user = null;
        Destroy(gameObject);
    }


}
