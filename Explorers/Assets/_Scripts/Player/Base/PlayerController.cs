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
    [HideInInspector]
    public int myIndex;

    private Rigidbody _rigidbody;
    
    [Header("�ƶ�")]
    public float speed;
    private Vector2 _inputDir;//���뷽��
    private Vector3 _moveDir;//�ƶ�����

    /// <summary>
    /// ��ʼ������
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
    /// ͨ����ȡPlayerInputSetting�н��ܵ��ķ��򣬺ϲ�������������ƶ�����_moveDir
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
