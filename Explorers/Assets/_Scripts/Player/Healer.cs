using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : PlayerController
{
    private bool isLeft = false;
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
        CheckKeys();
    }
    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            //�����������������
            case "ReconnectArea":
                if (!_hasConnected && playerInputSetting.GetCableButtonDown())
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
    //��ҽ�ߡ�
    public override void MainAttack()
    {
        foreach(var player in EnemyManager.Instance.players)
        {
            if (player.name == "BatteryCarrier") continue;
            //�ǵ�ؽ�ɫ�ظ�����
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.currentArmor = Mathf.Min(controller.currentArmor + mainWeapon.attackDamage, controller.maxArmor);
        }
    }

    //��֯
    public override void SecondaryAttack()
    {
        GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(secondaryWeapons, new Vector3(transform.localScale.x, 0, 0));
    }

}
