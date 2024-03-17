using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : Item
{
    List<GameObject> characters = new();
    public float activeTime = 3f;
    private float activeTimer = 0;
    public bool isUsing = false;
    public float force = 25f;
    public SphereCollider sphereCollider;
    public GameObject sprite;
    private Rigidbody rb;
    public bool isPicked;
    public override void Apply(GameObject user)
    {
        if (isPicked) return;
        rb.Sleep();
        isPicked = true;
        sprite.SetActive(false);
        
        
        user.GetComponent<PlayerController>().item = this;
        //Use(user);
    }
    public override void Use(GameObject user)
    {
        
        //todo ÈÓ³öÈ¥

        user.GetComponent<PlayerController>().item = null;
        Invoke("GravityChange", 0.5f);
    }
    private void Awake()
    {
        //GravityChange();
        sprite = transform.GetChild(0).gameObject;
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        
    }

    public void GravityChange()
    {
        sprite.SetActive(true);
        rb.WakeUp();
        sphereCollider.radius = 5f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isUsing = true;
        foreach(var character in characters)
        {
            character.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!isUsing) return;
        if (activeTimer >= activeTime)
        {
            activeTimer = 0;
            isUsing = false;
            foreach (var character in characters)
            {
                Vector3 forceDirection = transform.position - character.transform.position;
                if (character.CompareTag("Enemy"))
                {
                    character.GetComponent<Enemy>().canAttack = true;
                }
            }
        }
        else
        {
            activeTimer += Time.fixedDeltaTime;
            foreach(var character in characters)
            {
                Vector3 forceDirection = (transform.position - character.transform.position).normalized;
                if (character.CompareTag("Battery") || character.CompareTag("Player"))
                {
                    
                    character.GetComponent<Rigidbody>().AddForce(forceDirection * force*10f, ForceMode.Force);
                    
                }
                else
                {
                    
                    character.GetComponent<Enemy>().Vertigo(forceDirection * force*10f, ForceMode.Force, activeTime);
                    character.GetComponent<Enemy>().canAttack = false;
                }
            }



        }

    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Battery") || other.CompareTag("Player") || other.CompareTag("Enemy"))
            characters.Add(other.gameObject);
    }
   

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Battery") || other.CompareTag("Player") || other.CompareTag("Enemy"))
            characters.Remove(other.gameObject);
    }
}
