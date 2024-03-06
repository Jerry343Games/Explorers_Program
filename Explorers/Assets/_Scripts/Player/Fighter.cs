using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : PlayerController
{
    
    public float attackAngle = 90f;
    public LayerMask enemyLayer;
    private List<GameObject> _enemyInArea=new();
    
    // Start is called before the first frame update
    void Awake()
    {
        
        PlayerInit();   
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInputSetting.GetAttackButtonDown())
        {           
            PerformAttack();
        }
        if (hasDead) return;
        CharacterMove();
        if (playerInputSetting.inputDir.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        CheckDistanceToBattery();
        
    }

    private void OnTriggerStay(Collider other)
    {
        switch(other.tag)
        {
            //进入可重连绳子区域
            case "ReconnectArea":
                if(!_hasConnected /*&& 按下重连键*/)
                {
                    ReconnectRope();
                }
                break;
            //收集到场景物品
            case "Item":
                other.GetComponent<Item>().Apply(gameObject);
                break;
            default:
                break;
        }
    }
    public void PerformAttack()
    {
        if (_enemyInArea.Count == 0) return;
        for(int i = 0; i < _enemyInArea.Count; i++)
        {
            _enemyInArea[i].gameObject.GetComponent<Enemy>().TakeDamage(attack);
            _enemyInArea[i].gameObject.GetComponent<Enemy>().Vertigo(transform.right);
        }
    }

    private void OnTriggerEnter(Collider other)
    {       
        if (other.gameObject.CompareTag("Enemy"))
        {
            _enemyInArea.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("Enemy")&&_enemyInArea.Contains(other.gameObject))
        {
            _enemyInArea.Remove(other.gameObject);
        }
    }
    public override void Vertigo(Vector3 force)
    {
        
    }
}
