using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ʹ�ã�int��PlayerType.name ��ȡname��Ӧ�ı��
/// </summary>
public enum PlayerType
{
    BatteryCarrier=0,
    Shooter=3,
    Healer=2,
    Fighter=1
}


/// <summary>
/// ������Ʒ����
/// </summary>

public enum ItemType
{
    Collections,//�ռ���
    Battery,//�����еĵ��
    Boost,//����������
}

/// <summary>
/// �ռ�������
/// </summary>
public enum CollectionType
{
    Gold,
}
