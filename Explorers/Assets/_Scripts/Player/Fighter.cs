using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : PlayerController
{
    
    public float attackAngle = 90f;
    public LayerMask enemyLayer;
    private List<GameObject> _enemyInArea=new();
    private bool isLeft = false;
    public float force = 5f;
    // Start is called before the first frame update
    void Awake()
    {
        
        PlayerInit();   
    }
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
        if (playerInputSetting.GetAttackButtonDown())
        {           
            //PerformAttack();
            Attack();
        }
        CharacterMove();
        if (playerInputSetting.inputDir.x < 0)
        {
            transform.localScale = new(-1, 1, 1);
            isLeft = true;
        }
        else
        {
            isLeft = false;
            transform.localScale = new(1, 1, 1);
        }
        CheckDistanceToBattery();
        
    }

    private void OnTriggerStay(Collider other)
    {
        switch(other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if(!_hasConnected && playerInputSetting.GetCableButtonDown())
                {
                    ReconnectRope();
                }
                break;
            //�ռ���������Ʒ
            case "Item":
                other.GetComponent<Item>().Apply(gameObject);
                break;
            default:
                break;
        }
    }

    public override void MainAttack()
    {
        if (_enemyInArea.Count == 0) return;
        for (int i = 0; i < _enemyInArea.Count; i++)
        {
            _enemyInArea[i].GetComponent<Enemy>().TakeDamage(attack);
            if (isLeft)
            {
                _enemyInArea[i].GetComponent<Enemy>().Vertigo(-transform.right * force);
            }
            else
            {
                _enemyInArea[i].GetComponent<Enemy>().Vertigo(transform.right * force);
            }

        }
    }
    //public void PerformAttack()
    //{
    //    if (_enemyInArea.Count == 0) return;
    //    for(int i = 0; i < _enemyInArea.Count; i++)
    //    {
    //        _enemyInArea[i].GetComponent<Enemy>().TakeDamage(attack);
    //        if (isLeft)
    //        {
    //            _enemyInArea[i].GetComponent<Enemy>().Vertigo(-transform.right * force);
    //        }
    //        else
    //        {
    //            _enemyInArea[i].GetComponent<Enemy>().Vertigo(transform.right *force);
    //        }
           
    //    }
    //}

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
