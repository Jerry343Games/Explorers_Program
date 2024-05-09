using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff Store/Buff")]
public class UpgradeBuff : ScriptableObject
{
    public BuffType buffType;
    public string buffName;
    public string description;
    //public PlayerController playerController;
    public float value;
    public Sprite buffIcon;
}

public enum BuffType
{
    General,
    Explorers,
    BatteryCarrier,
    Shooter,
    Fighter,
    Healer
}
