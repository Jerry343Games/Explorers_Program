using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ʹ�ã�int��PlayerType.name ��ȡname��Ӧ�ı��
/// </summary>
public enum PlayerType
{
    BatteryCarrier=0,
    Shooter=1,
    Fighter = 2,
    Healer =3,
}

/// <summary>
/// ��������
/// </summary>
public enum BubbleType
{
    Press,
    Hold
}

public enum CharacterAnimation
{
    BatteryLeft_Walk,
    BatteryRight_Walk,
    BatteryLeft_Run,
    BatteryRight_Run,
    ShooterLeft_Idle,
    ShooterRight_Idle,
    ShooterLeft_Run,
    ShooterRight_Run,
    FighterLeft_Walk,
    FighterRight_Walk,
    FighterLeft_Run,
    FighterRight_Run,
    FighterLeft_Attack,
    FighterRight_Attack,
    HealerLeft_Idle,
    HealerRight_Idle,
    HealerLeft_Run,
    HealerRight_Run,

}

/// <summary>
/// ������Ʒ����
/// </summary>
public enum ItemType
{
    Resource,//�ռ���
    Battery,//�����еĵ��
    Boost,//����������
}

/// <summary>
/// �ռ�������
/// </summary>
public enum CollectionType
{
    PowerFurnaceParts,//����¯���
    Antenna,//����
    StorageAreaShell,//�ִ������
}

public enum ResourceType
{
    MineralCusters,//�����
}

/// <summary>
/// ��������
/// </summary>
public enum PropType
{
    GravityWell,
    PropelBackpack,
    Sonar,

}

