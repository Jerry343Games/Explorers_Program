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
    Fighter = 2,
    Healer =3,
}

/// <summary>
/// 气泡种类
/// </summary>
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
    PowerFurnaceParts,//动力炉零件
    Antenna,//天线
    StorageAreaShell,//仓储区外壳
}

public enum ResourceType
{
    MineralCusters,//矿物簇
}

/// <summary>
/// 道具类型
/// </summary>
public enum PropType
{


}

