using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniEventControl : MonoBehaviour
{
    public delegate void AniEventHandler();
    
    public event AniEventHandler OnFighterAttackEvent;

    public event AniEventHandler EndFighterAttackEvent;


    public event AniEventHandler OnEnemyAttackEvent;
    public event AniEventHandler EndEnemyAttackEvent;
    /// <summary>
    /// 触发OnAttack动画事件时
    /// </summary>
    public void OnFighterAttack()
    {
        OnFighterAttackEvent?.Invoke();
        
    }

    public void EndFighterAttack()
    {
        EndFighterAttackEvent?.Invoke();
    }
    /// <summary>
    /// 敌人开始攻击时调用，速度减慢到几乎为0
    /// </summary>
    public void OnEnemyAttack()
    {
        OnEnemyAttackEvent?.Invoke();
    }
    public void EndEnemyAttack()
    {
        EndEnemyAttackEvent?.Invoke();
    }

}
