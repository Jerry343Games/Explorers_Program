using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    /// <summary>
    /// 玩家类型
    /// </summary>
    public PlayerType playerType;
    
    /// <summary>
    /// 基础速度
    /// </summary>
    public float baseSpeed;
    
    /// <summary>
    /// 最大护甲
    /// </summary>
    public int maxArmor;
    
    /// <summary>
    /// 主武器
    /// </summary>
    public WeaponDataSO mainWeapon;
    
    /// <summary>
    /// 副武器
    /// </summary>
    public WeaponDataSO secondaryWeapon;

    public PlayerInfo(PlayerType playerType, float baseSpeed, int maxArmor,WeaponDataSO mainWeapon,WeaponDataSO secondaryWeapon)
    {
        this.playerType = playerType;
        this.baseSpeed = baseSpeed;
        this.maxArmor = maxArmor;
        this.mainWeapon = mainWeapon;
        this.secondaryWeapon = secondaryWeapon;
    }
    
    public override string ToString()
    {
        return string.Format("Player type: {0}\n BaseSpeed: {1}\n Max Armor: {2}\n MainWeapon: {3}\n SecondaryWeapon: {4}\n", 
            playerType,baseSpeed,maxArmor,mainWeapon,secondaryWeapon);
    }
}
