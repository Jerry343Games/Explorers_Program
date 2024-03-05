using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人的各自行为SO的父类，不同的敌人的data继承这个类后实现里面的方法，做到不同的行为模式
/// 下面的方法不一定都要用上
/// </summary>
public abstract class EnemySO : ScriptableObject
{



    public abstract void Move(Enemy enemy);

    public abstract void Attack(Enemy enemy);


    public abstract void Update();

    public abstract void TakeDamage();

    public abstract void FixUpdate();
}
