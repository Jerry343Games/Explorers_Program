using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���˵ĸ�����ΪSO�ĸ��࣬��ͬ�ĵ��˵�data�̳�������ʵ������ķ�����������ͬ����Ϊģʽ
/// ����ķ�����һ����Ҫ����
/// </summary>
public abstract class EnemySO : ScriptableObject
{



    public abstract void Move(Enemy enemy);

    public abstract void Attack(Enemy enemy);


    public abstract void Update();

    public abstract void TakeDamage();

    public abstract void FixUpdate();
}
