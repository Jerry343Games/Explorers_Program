using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private void Awake()
    {
        Init();
        
        //��ȡ�����ţ������Ӧ��ģ��
        switch (_playerInput.playerIndex)
        {
            case (int)PlayerType.BatteryCarrier:
                batteryCarrier.SetActive(true);
                break;
            case (int)PlayerType.Shooter:
                shooter.SetActive(true);
                break;
            case (int)PlayerType.Healer:
                healer.SetActive(true);
                break;
            case (int)PlayerType.Fighter:
                fighter.SetActive(true);
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
        _aim = inputActionAsset["Aim"];
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
    public bool GetInteractButtonDown() => _interact.WasPressedThisFrame();
    public bool GetInteractButtonRelease() => _interact.WasReleasedThisFrame();
    
    /// <summary>
    /// ��ǰ֡�Ƿ��¼��ټ���Space��East
    /// </summary>
    /// <returns></returns>
    public bool GetAccelerateButtonDown() => _accelerate.WasPressedThisFrame();
    public bool GetAccelerateButtonRelease() => _accelerate.WasReleasedThisFrame();
    
    /// <summary>
    /// ��ǰ֡�Ƿ�����������F��LT
    /// </summary>
    /// <returns></returns>
    public bool GetCableButtonDown() => _cableSetting.WasPressedThisFrame();
    public bool GetCableButtonRelease() => _cableSetting.WasReleasedThisFrame();
    
    /// <summary>
    /// ��ǰ֡�Ƿ��¹���������������X
    /// </summary>
    /// <returns></returns>
    public bool GetAttackButtonDown() => _attack.WasPressedThisFrame();
    public bool GetAttackButtonRelease() => _attack.WasReleasedThisFrame();
    
    /// <summary>
    /// ��ǰ֡�Ƿ�����׼��������Ҽ���RT
    /// </summary>
    /// <returns></returns>
    public bool GetAimButtonDown() => _aim.WasPressedThisFrame();
    public bool GetAimButtonRelease() => _aim.WasReleasedThisFrame();
}
