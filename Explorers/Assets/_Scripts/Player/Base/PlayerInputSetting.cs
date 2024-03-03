using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        switch (_playerInput.playerIndex)
        {
            case 0:
                batteryCarrier.SetActive(true);
                break;
            case 1:
                shooter.SetActive(true);
                break;
            case 2:
                healer.SetActive(true);
                break;
            case 3:
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
