using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 负责统一处理玩家的输入数据接收，并控制对应的玩家模块开启
/// </summary>
public class PlayerInputSetting : MonoBehaviour
{
    private PlayerInput _playerInput;
    
    [Header("玩家模块")]
    public GameObject batteryCarrier;
    public GameObject shooter;
    public GameObject healer;
    public GameObject fighter;

    [HideInInspector]
    public Vector2 inputDir;
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        
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

    /// <summary>
    /// 用于接收InputAction返回的玩家输入数据,玩家每次输入会被Call一次
    /// </summary>
    /// <param name="value0">输入数据</param>
    public void OnMovement(InputAction.CallbackContext value0)
    {
        inputDir = value0.ReadValue<Vector2>();
    }

}
