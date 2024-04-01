using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniEventControl : MonoBehaviour
{
    public delegate void AniEventHandler();
    
    public event AniEventHandler OnFighterAttackEvent;

    public event AniEventHandler EndFighterAttackEvent;

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
}
