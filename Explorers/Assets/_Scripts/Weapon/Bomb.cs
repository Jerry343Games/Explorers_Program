using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Bomb : MonoBehaviour
{
    private Rigidbody _rb;
    private SphereCollider _coll;

    private float _damage;
    private Vector3 _dir;
    private float _speed;
    private float _destroyTime;
    private float _destroyTimer;
    private float _prepareTime;
    private float _prepareTimer=0;
    private bool hasInit = false;
    private bool hasPrepare = false;
    List<GameObject> characters = new();
    public float force = 5f;
    private PlayerController _controller;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<SphereCollider>();
        
    }

    public void Init(WeaponDataSO data,Vector3 dir,float prepareTime = 1f, PlayerController controller=null)
    {
        _controller = controller;
        _damage = data.attackDamage;
        _speed = data.attackSpeed;
        _destroyTime = data.attackRange / 60f;
        _destroyTimer = 0;
        _dir = dir;
       _prepareTime = prepareTime;
        hasInit = true;
    }

    private void Update()
    {
        if (!hasInit) return;
        if (!hasPrepare&& _prepareTimer < _prepareTime)
        {
            _prepareTimer += Time.deltaTime;             
        }
        else
        {
            hasPrepare = true;
            _prepareTimer = 0;
        }
        if (hasPrepare)
        {
            Debug.Log(characters.Count);
            hasPrepare = false;
            
            foreach (var character in characters)
            {
                Debug.Log(character.name);
                character.GetComponent<Rigidbody>().velocity = Vector3.zero;
                Vector3 forceDirection = (character.transform.position - transform.position).normalized;
                if(character.GetComponent<Enemy>())
                {
                    character.GetComponent<Enemy>().Vertigo(forceDirection * force);
                    character.GetComponent<Enemy>().TakeDamage((int)_damage);
                }
                else
                {
                    GiantRockCrab.Instance.TakeDamage((int)_damage);
                }
            }
            (_controller as Fighter).hasUseBomb = false;
            MusicManager.Instance.PlaySound("��");
            GameObject effect = Instantiate(Resources.Load<GameObject>("Effect/RocketExplosion"), transform.position, Quaternion.identity);
            effect.transform.localScale *= 1;
            Destroy(gameObject);

        }
        
    }

    private void FixedUpdate()
    {
        _rb.velocity = _dir * _speed;

    }

    


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if ( other.CompareTag("Enemy") || other.CompareTag("Boss"))
            characters.Add(other.gameObject);
    }


    private void OnTriggerExit(Collider other)
    {
        if ( other.CompareTag("Enemy") || other.CompareTag("Boss"))
            characters.Remove(other.gameObject);
    }
}
