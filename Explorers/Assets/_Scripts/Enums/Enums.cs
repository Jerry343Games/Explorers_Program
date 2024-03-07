using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以使用（int）PlayerType.name 获取name对应的编号
/// </summary>
public enum PlayerType
{
    BatteryCarrier=0,
    Shooter=2,
    Healer=3,
    Fighter=1,
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
/// 零件类型
/// </summary>
public enum ResourceType
{
    Gold,
}


/// <summary>
/// 道具类型
/// </summary>
public enum PropType
{


}
