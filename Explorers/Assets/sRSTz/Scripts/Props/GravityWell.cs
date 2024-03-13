using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : Item
{
    List<GameObject> characters = new();
    public float activeTime = 3f;
    private float activeTimer = 0;
    public bool isUsing = false;
    public float force = 5f;

    public override void Apply(GameObject user)
    {
        



    }
    private void Awake()
    {
        //GravityChange();
        
    }

    public void GravityChange()
    {
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
                Vector3 forceDirection = transform.position - character.transform.position;
                if (character.CompareTag("Battery") || character.CompareTag("Player"))
                {
                    
                    character.GetComponent<PlayerController>().Vertigo(forceDirection * force, ForceMode.Force, activeTime);
                    
                }
                else
                {
                    
                    character.GetComponent<Enemy>().Vertigo(forceDirection * force, ForceMode.Force, activeTime);
                    character.GetComponent<Enemy>().canAttack = false;
                }
            }



        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery")) GravityChange();
        if (other.CompareTag("Battery") || other.CompareTag("Player") || other.CompareTag("Enemy"))
            characters.Add(other.gameObject);
    }
   

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Battery") || other.CompareTag("Player") || other.CompareTag("Enemy"))
            characters.Remove(other.gameObject);
    }
}
