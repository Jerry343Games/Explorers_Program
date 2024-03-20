using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fighter : PlayerController
{
    
    public float attackAngle = 90f;
    public LayerMask enemyLayer;
    private List<GameObject> _enemyInArea=new();
    private bool isLeft = false;
    public float force = 5f;
    
    public Transform shootTransform;
    public int tempArmor;//临时护盾
    public bool canFusionBomb = true;
    private Rigidbody _rb;
    
    void Awake()
    {
        
        _rb = GetComponent<Rigidbody>();
        PlayerInit();   
    }
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
        if (playerInputSetting.GetAttackButtonDown())
        {           
            Attack();
        }
        if(!isDashing) CharacterMove();

        RestroeDefence();
        if (!isDashing)
        {
            if (playerInputSetting.inputDir.x < 0)
            {
                transform.localScale = new(-1, 1, 1);
                isLeft = true;
            }
            else if (playerInputSetting.inputDir.x > 0)
            {
                isLeft = false;
                transform.localScale = new(1, 1, 1);
            }
        }
        //UseItem();
        CheckDistanceToBattery();
        CheckKeys();
        //冲刺相关
        if (isDashing)
        {
            _rb.angularVelocity = Vector3.zero;
            if (dashTimer < dashTime)
            {
                dashTimer += Time.deltaTime;
            }
            else
            {
                dashTimer = 0;
                isDashing = false;
                
            }
        }
        
        if (playerInputSetting.GetOptionalFeatureDown()&& Skill())
        {
             Dash(); 
        }
        UpdatSkillState();
    }
    

    private void OnTriggerStay(Collider other)
    {
        switch(other.tag)
        {
            //进入可重连绳子区域
            case "ReconnectArea":
                if (!_hasConnected)
                {
                    if (playerInputSetting.GetCableButtonDown())
                    {
                        ReconnectRope();
                        
                        //重连后销毁重连提示气泡
                        bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                    }
                }
                break;
            //收集到场景物品
            case "Item":
                if((other.transform.position-transform.position).magnitude<0.3f)
                other.GetComponent<Item>().Apply(gameObject);
                break;
            case "Resource":
                if(!isDigging&& playerInputSetting.GetInteractButtonDown())
                {
                    isDigging = true;
                    _curDigRes = other.GetComponent<Resource>();
                   

                }
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        CreatBubbleUI(other.gameObject);
        if (other.gameObject.CompareTag("Enemy"))
        {
            _enemyInArea.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {

        switch (other.tag)
        {
            case "Resource":
                if (isDigging)
                {
                    isDigging = false;
                   
                    _curDigRes = null;
                }
                //离开资源区域后销毁交互气泡
                bubblePanel.interectBubbleBuffer.GetComponent<UIBubbleItem>().DestoryBubble();
                break;
            case "Enemy":
                if(_enemyInArea.Contains(other.gameObject))
                {
                    _enemyInArea.Remove(other.gameObject);
                }
                break;
            case "ReconnectArea":
                //离开重连区域后如果有重连气泡就销毁下
                if (bubblePanel.reconnectCableBuffer)
                {
                    bubblePanel.reconnectCableBuffer.GetComponent<UIBubbleItem>().DestoryBubble();    
                }
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
            _enemyInArea[i].GetComponent<Enemy>().TakeDamage((int)currentWeapon.attackDamage);
            if (_enemyInArea[i].GetComponent<Enemy>().HP <= 0) _enemyInArea.RemoveAt(i);
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

    public override void SecondaryAttack()
    {
        GameObject bomb = Instantiate(Resources.Load<GameObject>("Bomb"), transform.position, Quaternion.identity);
        bomb.GetComponent<Bomb>().Init(secondaryWeapons, new Vector3(transform.localScale.x, 0, 0));
    }

    //因为超载可以获得临时护盾 所以重写一下受伤方法
    public override void TakeDamage(int damage)
    {
        if (hasDead||isDashing) return;
        if (isDigging)
        {
            isDigging = false;//打断状态
           
        }
        int realDamage = damage;
        if(damage<=tempArmor)
        {
            tempArmor-=damage;

        }
        else
        {
            realDamage = damage - tempArmor;
            tempArmor = 0;
        }

        if (realDamage < currentArmor)
        {
            currentArmor -= realDamage;
        }
        else
        {
            int damageToBattery = realDamage - currentArmor;
            GetComponent<Battery>().ChangePower(-damageToBattery);
            currentArmor = maxArmor;
        }
    }
    

    //自选功能
    private bool isDashing = false;
    public float dashTime = 3f;
    private float dashTimer = 0;
    public float dashForce = 1f;
    public int dashDamage = 15;
    public void Dash()
    {
        Vector3 moveDir = new Vector3(playerInputSetting.inputDir.x, playerInputSetting.inputDir.y, 0).normalized;
        if (moveDir.Equals(Vector3.zero)) return;
        isDashing = true;        
        _rb .AddForce(moveDir * dashForce, ForceMode.Impulse);

    }

    public void FusionBomb()
    {

    }
    public void OnCollisionEnter(Collision collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Enemy")){
            collision.gameObject.GetComponent<Enemy>().TakeDamage(dashDamage);
        }
    }

}
