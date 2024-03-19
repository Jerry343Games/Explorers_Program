using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以使用（int）PlayerType.name 获取name对应的编号
/// </summary>
public enum PlayerType
{
    BatteryCarrier=0,
    Shooter=1,
    Healer=2,
    Fighter=3,
}

public enum BubbleType
{
    ResourceCollectionBubble,
    ReconnectCableBubble,
}

/// <summary>
/// 场景物品类型
/// </summary>
public enum ItemType
{
    Resource,//收集物
    Battery,//场景中的电池
    Boost,//属性提升？
}

/// <summary>
/// 收集物类型
/// </summary>
public enum CollectionType
{
    Screw,//螺丝
    Tin,//铁皮
    //and so on
}

public enum ResourceType
{
    Gold,
    Nickel,
    RareEarth,
}

/// <summary>
/// 道具类型
/// </summary>
public enum PropType
{


}

