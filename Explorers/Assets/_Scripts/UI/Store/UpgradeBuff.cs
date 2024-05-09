using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff Store/Buff")]
public class UpgradeBuff : ScriptableObject
{
    public BuffType buffType;
    public string buffName;
    public string description;
    public PlayerController playerController;
    public float value;
    public Sprite buffIcon;
}

public enum BuffType
{
    General,//通用
    Explorers,//探索
    Battery,//电池
    Shooter,//射手
    Fighter,//战士
    Healer//医疗
}
