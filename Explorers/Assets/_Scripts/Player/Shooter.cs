using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : PlayerController
{
    private bool isLeft;
    void Awake()
    {
        PlayerInit();
    }
    // Update is called once per frame
    void Update()
    {
        if (hasDead) return;
        UpdateAttackState();
        if (playerInputSetting.GetAttackButtonDown())
        {
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
        switch (other.tag)
        {
            //进入可重连绳子区域
            case "ReconnectArea":
                if (!_hasConnected && playerInputSetting.GetCableButtonDown())
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

    //歼灭者
    public override void MainAttack()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(mainWeapon, new Vector3(transform.localScale.x, 0, 0));
    }

    //鱼叉
    public override void SecondaryAttack()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(secondaryWeapons, new Vector3(transform.localScale.x, 0, 0));
    }
}
