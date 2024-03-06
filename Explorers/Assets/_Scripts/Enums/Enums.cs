using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以使用（int）PlayerType.name 获取name对应的编号
/// </summary>
public enum PlayerType
{
    BatteryCarrier=0,
    Shooter=3,
    Healer=2,
    Fighter=1
}


/// <summary>
/// 场景物品类型
/// </summary>

public enum ItemType
{
    Collections,//收集物
    Battery,//场景中的电池
    Boost,//属性提升？
}

/// <summary>
/// 收集物类型
/// </summary>
public enum CollectionType
{
    Gold,
}
