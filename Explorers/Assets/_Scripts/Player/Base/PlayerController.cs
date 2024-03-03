using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerInputSetting _playerInputSetting;
    
    /// <summary>
    /// 玩家所分配到的唯一序列号,类型区分见枚举类
    /// </summary>
    [HideInInspector]
    public int myIndex;

    private Rigidbody _rigidbody;
    
    [Header("移动")]
    public float speed;
    private Vector2 _inputDir;//输入方向
    private Vector3 _moveDir;//移动方向

    /// <summary>
    /// 初始化方法
    /// </summary>
    public void PlayerInit()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = transform.parent.GetComponent<PlayerInput>();
        _playerInputSetting = transform.parent.GetComponent<PlayerInputSetting>();
        myIndex = _playerInput.playerIndex;
        Debug.Log(transform.name+" Index: "+myIndex);
    }
    
    
    /// <summary>
    /// 通过获取PlayerInputSetting中接受到的方向，合并输入向量获得移动方向_moveDir
    /// </summary>
    public void MovementCombination()
    {
        _moveDir=new Vector3(_playerInputSetting.inputDir.x, _playerInputSetting.inputDir.y,0).normalized;
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
