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
    Healer=3,
    Fighter=2,
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
public enum ResourceType
{
    Gold,
}
