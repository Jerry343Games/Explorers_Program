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
    /// ��������䵽��Ψһ���к�,�������ּ�ö����
    /// </summary>
    private float myIndex;
    
    private Vector2 _inputDir;//���뷽��
    private Vector3 _moveDir;//�ƶ�����

    private Rigidbody _rigidbody;

    public float speed;

    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void PlayerInit()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = transform.parent.GetComponent<PlayerInput>();
        _playerInputSetting = transform.parent.GetComponent<PlayerInputSetting>();
        myIndex = _playerInput.playerIndex;
        Debug.Log(transform.name+": "+myIndex);
    }
    
    
    /// <summary>
    /// ͨ����ȡPlayerInputSetting�н��ܵ��ķ��򣬺ϲ�������������ƶ�����
    /// </summary>
    public void MovementCombination()
    {
        _moveDir=new Vector3(_playerInputSetting.inputDir.x, _playerInputSetting.inputDir.y,0).normalized;
    }
    
    /// <summary>
    /// ��ɫ�ƶ�����
    /// </summary>
    public void CharacterMove()
    {
        MovementCombination();
        transform.Translate(_moveDir * Time.deltaTime * speed, Space.World);
    }
    
}
