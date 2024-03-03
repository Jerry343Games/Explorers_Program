using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    
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
    }

    /// <summary>
    /// ���ڽ���InputAction���ص������������,���ÿ������ᱻCallһ��
    /// </summary>
    /// <param name="value0">��������</param>
    public void OnMovement(InputAction.CallbackContext value0)
    {
        _inputDir = value0.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// �ϲ�������������ƶ�����
    /// </summary>
    public void MovementCombination()
    {
        _moveDir=new Vector3(_inputDir.x, _inputDir.y,0).normalized;
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
