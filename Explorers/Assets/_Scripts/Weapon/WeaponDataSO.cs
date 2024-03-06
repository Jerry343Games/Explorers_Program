using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponData",menuName = "WeaponData/New WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    public string weaponName;//������
    [TextArea(3,5)]
    public string weaponDescription;//��������
    public int attackDamage;//������
    public float attackRange;//������Χ
    public float attackCD;//������ȴ
    public float attackSpeed;//�ӵ�/Ͷ�����ٶ�
    public int initAmmunition;//��ʼ��ҩ��
}
