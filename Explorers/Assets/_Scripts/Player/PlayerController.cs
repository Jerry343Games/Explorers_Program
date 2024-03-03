using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    
    private Vector2 _inputDir;//输入方向
    private Vector3 _moveDir;//移动方向

    private Rigidbody _rigidbody;

    public float speed;

    /// <summary>
    /// 初始化方法
    /// </summary>
    public void PlayerInit()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 用于接收InputAction返回的玩家输入数据,玩家每次输入会被Call一次
    /// </summary>
    /// <param name="value0">输入数据</param>
    public void OnMovement(InputAction.CallbackContext value0)
    {
        _inputDir = value0.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// 合并输入向量获得移动方向
    /// </summary>
    public void MovementCombination()
    {
        _moveDir=new Vector3(_inputDir.x, _inputDir.y,0).normalized;
    }
    
    /// <summary>
    /// 角色移动方法
    /// </summary>
    public void CharacterMove()
    {
        MovementCombination();
        transform.Translate(_moveDir * Time.deltaTime * speed, Space.World);
    }
    
}
