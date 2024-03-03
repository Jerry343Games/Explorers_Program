using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSetting : MonoBehaviour
{
    private PlayerInput _playerInput;
    [Header("���ģ��")]
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
    /// ���ڽ���InputAction���ص������������,���ÿ������ᱻCallһ��
    /// </summary>
    /// <param name="value0">��������</param>
    public void OnMovement(InputAction.CallbackContext value0)
    {
        inputDir = value0.ReadValue<Vector2>();
    }
    
    

}
