using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

/// <summary>
/// 负责统一处理玩家的输入数据接收，并控制对应的玩家模块开启
/// </summary>
public class PlayerInputSetting : MonoBehaviour
{
    //处理输入
    private PlayerInput _playerInput;
    public InputActionAsset inputActionAsset;
    private InputAction _aim;
    public string myControlScheme;
    
    [Header("玩家模块")]
    public GameObject batteryCarrier;
    public GameObject shooter;
    public GameObject healer;
    public GameObject fighter;
    public PlayerType characterType;
    public OptionalFeature feature;//选择的技能
    
    [HideInInspector]
    public Vector2 inputDir;
    [HideInInspector]
    public Vector2 aimPos;//最终瞄准位置，用于获取
    private Vector2 _inputAimDir;
    private RaycastHit _hit;
    public LayerMask mouseRayLayer;
    private GameObject _player;
    public bool isStick;
    
    private bool isCharacterLock;
    private void Awake()
    {
        Init();
        DontDestroyOnLoad(gameObject);
        //TestScenePlayerSelect();//当不是从选择关卡开始时分配默认玩家
        myControlScheme = _playerInput.currentControlScheme;
    }

    private void Start()
    {
        PlayerManager.Instance?.RegisterPlayer(gameObject);
    }

    private void Init()
    {
        //归零父物体(出生点)
        transform.position = Vector3.zero;
        _playerInput = GetComponent<PlayerInput>();
        
        isCharacterLock = false;
        CharacterItem.OnPlayerTypeChanged += HandlePlayerTypeChanged;
        EventCenter.GameStartedEvent += FeatureAssignment;
    }
    private void OnDestroy()
    {
        CharacterItem.OnPlayerTypeChanged -= HandlePlayerTypeChanged;

        EventCenter.GameStartedEvent -= FeatureAssignment;

    }
    /// <summary>
    /// 外部获取选择的玩家职业
    /// </summary>
    /// <param name="myType">职业</param>
    private void HandlePlayerTypeChanged(PlayerType myType)
    {
        if (!isCharacterLock)
        {
            characterType = myType;
            //通过SelectPanel的外部传参设置预制体
            switch (myType)
            {
                case PlayerType.BatteryCarrier:
                    batteryCarrier.SetActive(true);
                    _player = batteryCarrier;
                    break;
                case PlayerType.Shooter:
                    shooter.SetActive(true);
                    _player = shooter;
                    break;
                case PlayerType.Healer:
                    healer.SetActive(true);
                    _player = healer;
                    break;
                case PlayerType.Fighter:
                    fighter.SetActive(true);
                    _player = fighter;
                    break;
            }

            isCharacterLock = true;
        }
    }

    /// <summary>
    /// 用于在不经过选人关卡时分配角色
    /// </summary>
    //private void TestScenePlayerSelect()
    //{
    //    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "SelectScene")
    //    {
    //        switch (_playerInput.playerIndex)
    //        {
    //            case (int)PlayerType.BatteryCarrier:
    //                batteryCarrier.SetActive(true);
    //                _player = batteryCarrier;
    //                break;
    //            case (int)PlayerType.Shooter:
    //                shooter.SetActive(true);
    //                _player = shooter;
    //                break;
    //            case (int)PlayerType.Healer:
    //                healer.SetActive(true);
    //                _player = healer;
    //                break;
    //            case (int)PlayerType.Fighter:
    //                fighter.SetActive(true);
    //                _player = fighter;
    //                break;
    //        }
    //    }
    //}

    public void SwitchToPlayerScheme()
    {
        _playerInput.SwitchCurrentActionMap("Player");
    }

    public void SwitchToUISchemeAndSelect(GameObject gameObject)
    {
        _playerInput.SwitchCurrentActionMap("UI");
        GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(gameObject);
    }

    public void SwitchToScheme(string name)
    {
        _playerInput.SwitchCurrentActionMap(name);
    }
    
    /// <summary>
    /// 游戏开始时统一进行自选功能分配
    /// </summary>
    private void FeatureAssignment()
    {
        Debug.Log(characterType);
        feature = PlayerManager.Instance.playerFeaturesDic[(int)characterType];
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
    /// 当前帧是否按下攻击键：鼠标左键，右肩键
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

    /// <summary>
    /// 副武器攻击：鼠标右键，左肩键
    /// </summary>
    private bool _isAttackSecondary;

    public void OnAttackSecondary(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isAttackSecondary = true;
        }
        else if (context.canceled)
        {
            _isAttackSecondary = false;
        }
    }
    public bool GetAttackSecondaryDown() => _isAttackSecondary;

    /// <summary>
    /// 自选功能 键盘Q，手柄右扳机键
    /// </summary>
    private bool _isOptionalFeature;
    public void OnOptionalFeeatureDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isOptionalFeature = true;
        }
        else if (context.canceled)
        {
            _isOptionalFeature = false;
        }
    }
    public bool GetOptionalFeatureDown() => _isOptionalFeature;

    /// <summary>
    /// 道具使用
    /// </summary>
    private bool _isUseItem;
    public void OnUseItemDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isUseItem = true;
        }else if (context.canceled)
        {
            _isUseItem = false;
        }
    }
    public bool GetUseItem() => _isUseItem;
    
    /// <summary>
    /// 道具使用
    /// </summary>
    private bool _isPressPause;
    public void OnPauseDown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isPressPause = true;
        }else if (context.canceled)
        {
            _isPressPause = false;
        }
    }
    public bool GetPressPause() => _isPressPause;

    /// <summary>
    /// 获取选择的向量
    /// </summary>
    private Vector3 _selectIndex;
    public void OnBatterySelect(InputAction.CallbackContext context)
    {
        _selectIndex = context.ReadValue<Vector3>();
    }
    public Vector3 GetSelectCombination() => _selectIndex;
    
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
        if (Physics.Raycast(ray, out _hit, 50, mouseRayLayer)) //如果碰撞检测到物体
        {
            _hitPos = new Vector2(_hit.point.x, _hit.point.y);
        }

        aimPos = _hitPos;
    }
}
