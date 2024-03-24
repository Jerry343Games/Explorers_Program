using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
public class PlayerManager : SingletonPersistent<PlayerManager>
{
    public List<GameObject> players = new List<GameObject>();

    //键：角色索引 值：选择的技能
    public Dictionary<int, OptionalFeature> playerFeaturesDic = new Dictionary<int, OptionalFeature>();

}
