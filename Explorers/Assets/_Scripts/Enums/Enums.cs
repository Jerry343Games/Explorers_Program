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
    ResourceCollectionBubble,
    ReconnectCableBubble,
}

public enum CharacterAnimation
{
    BatteryWalk,
    BatteryRun,
    ShooterIdle,
    ShooterRun,
    FighterWalk,
    FighterRun,
    FighterAttack,
    HealerIdle,
    HealerRun,
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

