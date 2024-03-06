using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData",menuName = "WeaponData/New WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    public string weaponName;//ÎäÆ÷Ãû
    [TextArea(3,5)]
    public string weaponDescription;//ÎäÆ÷ÃèÊö
    public int attackDamage;//¹¥»÷Á¦
    public float attackRange;//¹¥»÷·¶Î§
    public float attackCD;//¹¥»÷ÀäÈ´
    public float attackSpeed;//×Óµ¯/Í¶ÖÀÎïËÙ¶È
    public int initAmmunition;//³õÊ¼µ¯Ò©Á¿
}
