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
    Healer=2,
    Fighter=3
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
