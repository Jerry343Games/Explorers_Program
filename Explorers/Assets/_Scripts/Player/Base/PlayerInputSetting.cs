using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 负责统一处理玩家的输入数据接收，并控制对应的玩家模块开启
/// </summary>
public class PlayerInputSetting : MonoBehaviour
{
    //处理输入
    private PlayerInput _playerInput;
    public InputActionAsset inputActionAsset;
    private InputAction _interact;
    private InputAction _accelerate;
    private InputAction _cableSetting;
    private InputAction _attack;
    private InputAction _aim;
    
    [Header("玩家模块")]
    public GameObject batteryCarrier;
    public GameObject shooter;
    public GameObject healer;
    public GameObject fighter;

    [HideInInspector]
    public Vector2 inputDir;
    private void Awake()
    {
        Init();
        
        //读取玩家序号，分配对应的模块
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
        //归零父物体(出生点)
        transform.position = Vector3.zero;
        _playerInput = GetComponent<PlayerInput>();
        
        //获取输入配置文件
        _interact = inputActionAsset["Interact"];
        _cableSetting = inputActionAsset["CableSetting"];
        _accelerate = inputActionAsset["Accelerate"];
        _aim = inputActionAsset["Aim"];
        _attack = inputActionAsset["Attack"];
    }

    /// <summary>
    /// 用于接收InputAction返回的玩家输入数据,玩家每次输入会被Call一次
    /// </summary>
    /// <param name="value0">输入数据</param>
    public void OnMovement(InputAction.CallbackContext value0)
    {
        inputDir = value0.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// 当前帧是否按下交互键：E，North
    /// </summary>
    /// <returns></returns>
    private bool _isPressInteract;
    public void OnInteract(InputAction.CallbackContext context)
    {
        // 检测按键被按下
        if (context.started)
        {
            _isPressInteract = true;
        }
        // 检测按键被抬起
        else if (context.canceled)
        {
            _isPressInteract = false;
        }
    }
    public bool GetInteractButtonDown() => _isPressInteract;

    /// <summary>
    /// 当前帧是否按下加速键：Space，East
    /// </summary>
    /// <returns></returns>
    private bool _isPressAccelerate;
    public void OnAccelerate(InputAction.CallbackContext context)
    {
        // 检测按键被按下
        if (context.started)
        {
            _isPressAccelerate = true;
        }
        // 检测按键被抬起
        else if (context.canceled)
        {
            _isPressAccelerate = false;
        }
    }
    public bool GetAccelerateButtonDown() => _isPressAccelerate;

    /// <summary>
    /// 当前帧是否按下绳索键：F，LT
    /// </summary>
    /// <returns></returns>
    private bool _isPressCable;
    public void OnCable(InputAction.CallbackContext context)
    {
        // 检测按键被按下
        if (context.started)
        {
            _isPressCable = true;
        }
        // 检测按键被抬起
        else if (context.canceled)
        {
            _isPressCable = false;
        }
    }
    public bool GetCableButtonDown() => _isPressCable;

    /// <summary>
    /// 当前帧是否按下攻击键：鼠标左键，X
    /// </summary>
    /// <returns></returns>
    private bool _isPressAttack;
    public void OnAttack (InputAction.CallbackContext context)
    {
        // 检测按键被按下
        if (context.started)
        {
            _isPressAttack = true;
        }
        // 检测按键被抬起
        else if (context.canceled)
        {
            _isPressAttack = false;
        }
    }
    public bool GetAttackButtonDown() => _isPressAttack;
    
}
