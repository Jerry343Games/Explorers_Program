using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

/// <summary>
/// ����ͳһ������ҵ��������ݽ��գ������ƶ�Ӧ�����ģ�鿪��
/// </summary>
public class PlayerInputSetting : MonoBehaviour
{
    //��������
    private PlayerInput _playerInput;
    public InputActionAsset inputActionAsset;
    private InputAction _interact;
    private InputAction _accelerate;
    private InputAction _cableSetting;
    private InputAction _attack;
    private InputAction _aim;
    
    [Header("���ģ��")]
    public GameObject batteryCarrier;
    public GameObject shooter;
    public GameObject healer;
    public GameObject fighter;

    [HideInInspector]
    public Vector2 inputDir;
    [HideInInspector]
    public Vector2 aimPos;//������׼λ�ã����ڻ�ȡ
    private Vector2 _inputAimDir;
    private RaycastHit _hit;
    public LayerMask mouseRayLayer;
    private GameObject _player;
    public bool isStick;
    private void Awake()
    {
        Init();
        
        //��ȡ�����ţ������Ӧ��ģ��
        switch (_playerInput.playerIndex)
        {
            case (int)PlayerType.BatteryCarrier:
                batteryCarrier.SetActive(true);
                _player = batteryCarrier;
                break;
            case (int)PlayerType.Shooter:
                shooter.SetActive(true);
                _player = shooter;
                break;
            case (int)PlayerType.Healer:
                healer.SetActive(true);
                _player = healer;
                break;
            case (int)PlayerType.Fighter:
                fighter.SetActive(true);
                _player = fighter;
                break;
        }
    }

    private void Init()
    {
        //���㸸����(������)
        transform.position = Vector3.zero;
        _playerInput = GetComponent<PlayerInput>();
        
        //��ȡ���������ļ�
        _interact = inputActionAsset["Interact"];
        _cableSetting = inputActionAsset["CableSetting"];
        _accelerate = inputActionAsset["Accelerate"];
        _attack = inputActionAsset["Attack"];
    }

    /// <summary>
    /// ���ڽ���InputAction���ص������������,���ÿ������ᱻCallһ��
    /// </summary>
    /// <param name="value0">��������</param>
    public void OnMovement(InputAction.CallbackContext value0)
    {
        inputDir = value0.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// ��ǰ֡�Ƿ��½�������E��North
    /// </summary>
    /// <returns></returns>
    private bool _isPressInteract;
    public void OnInteract(InputAction.CallbackContext context)
    {
        // ��ⰴ��������
        if (context.started)
        {
            _isPressInteract = true;
        }
        // ��ⰴ����̧��
        else if (context.canceled)
        {
            _isPressInteract = false;
        }
    }
    public bool GetInteractButtonDown() => _isPressInteract;

    /// <summary>
    /// ��ǰ֡�Ƿ��¼��ټ���Space��East
    /// </summary>
    /// <returns></returns>
    private bool _isPressAccelerate;
    public void OnAccelerate(InputAction.CallbackContext context)
    {
        // ��ⰴ��������
        if (context.started)
        {
            _isPressAccelerate = true;
        }
        // ��ⰴ����̧��
        else if (context.canceled)
        {
            _isPressAccelerate = false;
        }
    }
    public bool GetAccelerateButtonDown() => _isPressAccelerate;

    /// <summary>
    /// ��ǰ֡�Ƿ�����������F��LT
    /// </summary>
    /// <returns></returns>
    private bool _isPressCable;
    public void OnCable(InputAction.CallbackContext context)
    {
        // ��ⰴ��������
        if (context.started)
        {
            _isPressCable = true;
        }
        // ��ⰴ����̧��
        else if (context.canceled)
        {
            _isPressCable = false;
        }
    }
    public bool GetCableButtonDown() => _isPressCable;

    /// <summary>
    /// ��ǰ֡�Ƿ��¹���������������X
    /// </summary>
    /// <returns></returns>
    private bool _isPressAttack;
    public void OnAttack (InputAction.CallbackContext context)
    {
        // ��ⰴ��������
        if (context.started)
        {
            _isPressAttack = true;
        }
        // ��ⰴ����̧��
        else if (context.canceled)
        {
            _isPressAttack = false;
        }
    }
    public bool GetAttackButtonDown() => _isPressAttack;

    public void OnStickAim(InputAction.CallbackContext value)
    {
        isStick = true;
        _inputAimDir = value.ReadValue<Vector2>().normalized;
        aimPos = _inputAimDir;
    }

    public void OnMouseAim(InputAction.CallbackContext value)
    {
        _inputAimDir = value.ReadValue<Vector2>();
        
        Ray ray = Camera.main.ScreenPointToRay(_inputAimDir);
        Vector2 _hitPos=Vector2.zero;
        if (Physics.Raycast(ray, out _hit, 50, mouseRayLayer)) //�����ײ��⵽����
        {
            _hitPos = new Vector2(_hit.point.x, _hit.point.y);
        }

        aimPos = _hitPos;
    }
}
