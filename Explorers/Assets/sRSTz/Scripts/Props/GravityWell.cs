using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GravityWell : PropItem
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
    public float throwPower=8f;
    private Vector3 userAimPos;

    public GameObject blackHoleEffect;
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

        MusicManager.Instance.PlaySound("引力井");
        sprite.SetActive(true);
        transform.position = user.transform.position;
        userAimPos = new Vector3(user.GetComponent<PlayerController>(). playerInputSetting.aimPos.x, user.GetComponent<PlayerController>().playerInputSetting.aimPos.y, 0);
        // 计算扔出的方向

        Vector2 direction = user.transform.GetChild(0).transform.forward;  /*(userAimPos - transform.position).normalized;*/
        rb.AddForce(direction * throwPower, ForceMode.Impulse);
        user.GetComponent<PlayerController>().item = null;
        Invoke("GravityChange", 1f);
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
        PlayBlackHoleAni();
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
            KillBlackHole();
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

    /// <summary>
    /// 开启黑洞特效
    /// </summary>
    private void PlayBlackHoleAni()
    {
        sprite.SetActive(false);
        Vector3 defaultScale = blackHoleEffect.transform.localScale;
        blackHoleEffect.transform.localScale = Vector3.zero;
        blackHoleEffect.transform.DOScale(defaultScale, 0.2f);
        blackHoleEffect.SetActive(true);
        blackHoleEffect.GetComponent<ParticleSystem>().Play();
    }

    /// <summary>
    /// 关闭黑洞
    /// </summary>
    private void KillBlackHole()
    {
        blackHoleEffect.transform.DOScale(Vector3.zero, 0.2f).OnComplete(()=>Destroy(gameObject));
    }
}
