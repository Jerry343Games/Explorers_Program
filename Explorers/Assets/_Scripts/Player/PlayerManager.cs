using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum OptionalFeature
{
    Logistics,//电池机器人功能 后勤
    Charging,//电池机器人功能 充能
    Salvo,//攻击型机器人功能 齐射
    DestroyTorpedoes,//攻击型机器人功能 毁灭鱼雷
    Dash,//防御型机器人功能 冲撞
    SonicWave,//防御型机器人功能 次声波
    TranquilizerGun,//治疗型机器人功能 麻醉枪
    FloatingFort,//治疗型机器人功能 浮游炮台
}

[Serializable]
public class AnimationTextureMapping
{
    public CharacterAnimation animationName;
    public Texture2D texture;
}
public class PlayerManager : SingletonPersistent<PlayerManager>
{
    public int maxPlayerCount;
    //游玩的玩家列表 是父物体
    public List<GameObject> players = new List<GameObject>();

    //键：职业索引 值：选择的技能
    public Dictionary<int, OptionalFeature> playerFeaturesDic = new Dictionary<int, OptionalFeature>();

    //预加载法线数据
    public List<AnimationTextureMapping> mappings = new List<AnimationTextureMapping>();

    public bool hasMainBattary;
    //敌人需要的敌人列表 是玩家角色子物体
    public List<GameObject> gamePlayers = new List<GameObject>();

    //通过动画查找法线
    public Texture2D GetTextureByAnimationName(CharacterAnimation animationName)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.animationName == animationName)
            {
                return mapping.texture;
            }
        }
        return null; // 如果找不到对应的贴图，则返回null
    }

    /// <summary>
    /// 注册玩家
    /// </summary>
    /// <param name="player"></param>
    public void RegisterPlayer(GameObject player)
    {
        Debug.Log("Register Player");
        players.Add(player);
    }

    /// <summary>
    /// 获得最后一个注册的玩家
    /// </summary>
    /// <returns></returns>
    public GameObject GetLatestPlayer()
    {
        if (players.Count > 0)
        {
            Debug.Log("GetLastPlayer");
            return players[players.Count - 1];
        }

        return null;
    }

    /// <summary>
    /// 销毁所有玩家
    /// </summary>
    public void DestoryAllPlayers()
    {
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
        players.Clear();
    }

}
