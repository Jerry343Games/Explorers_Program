using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CharacterItem : MonoBehaviour
{
    public UISelectPanel uiSelectPanel;

    //配装面板起止位置、动画时间
    private float _weaponInfoItemEndTop = 128f;
    private float _weaponInfoItemStartTop = 292f;
    private float _weaponInfoItemShowDuration = 0.3f;
    //角色信息起止位置、动画时间
    private float _infoItemEndTop = 58f;
    private float _infoItemStartTop = 97f;
    private float _infoItemHideDuration = 0.3f;
    
    public GameObject playerEventCollection;
    public bool isSelected;
    private Button _characterBtn;
    private Image _weaponSelectItem;
    public GameObject weaponInfoItem;
    public GameObject infoItem;
    private Image _infoItemImg;
    public static int playerIndex;

    [Header("指示灯")]
    public Image confirmLightImg;
    public Color unSelectColor;
    public Color selectedColor;
    public Color confirmedColor;
    // Start is called before the first frame update
    void Start()
    {
        _characterBtn = GetComponent<Button>();
        _infoItemImg = infoItem.GetComponent<Image>();
        _weaponSelectItem = transform.GetChild(0).GetComponent<Image>();
        _characterBtn.onClick.AddListener(ClickCharacterBtn);
        weaponInfoItem.GetComponent<OptionalFeatureItem>().FeatureConfirmEvent += CloseWeaponInfoItem;
        confirmLightImg.color = unSelectColor;
    }
    
    /// <summary>
    /// 处理角色面板的点击事件
    /// </summary>
    private void ClickCharacterBtn()
    {
        if (!isSelected)
        {
            confirmLightImg.color = selectedColor;
            isSelected = true;
            weaponInfoItem.transform.GetChild(0).GetComponent<Button>().Select();
            MultiplayerEventSystem eventSystem = playerEventCollection.transform.GetChild(playerIndex)
                .GetComponent<MultiplayerEventSystem>();
            eventSystem.playerRoot = weaponInfoItem;
            eventSystem.firstSelectedGameObject = weaponInfoItem.transform.GetChild(0).gameObject;
            
            //处理动画
            Sequence q = DOTween.Sequence();
            q.Append(HideInfoItem);
            q.Append(ShowWeaponItem);
            
            //允许添加下一个输入设备
            playerIndex++;
            if (playerIndex < uiSelectPanel.sceneManager.maxPlayer)
            {
                playerEventCollection.transform.GetChild(playerIndex).gameObject.SetActive(true);
            }
        }
    }
    
    /// <summary>
    /// 关闭武器选择
    /// </summary>
    private void CloseWeaponInfoItem()
    {
        confirmLightImg.color = confirmedColor;
        //处理动画
        Sequence q = DOTween.Sequence();
        q.Append(HideWeaponItem);
        q.Append(ShowInfoItem);
    }
    
    /// <summary>
    /// 显示武器面板动画
    /// </summary>
    private Tweener ShowWeaponItem=>DOTween.To(value =>
        {
            _weaponSelectItem.rectTransform.offsetMax =
                new Vector2(_weaponSelectItem.rectTransform.offsetMax.x, value);
        },
        _weaponSelectItem.rectTransform.offsetMax.y,
        -_weaponInfoItemEndTop,
        _weaponInfoItemShowDuration
    );
    
    /// <summary>
    /// 显示角色介绍面板动画
    /// </summary>
    private Tweener ShowInfoItem=>DOTween.To(value =>
        {
            _infoItemImg.rectTransform.offsetMax =
                new Vector2(_infoItemImg.rectTransform.offsetMax.x, value);
        },
        _infoItemImg.rectTransform.offsetMax.y,
        _infoItemStartTop,
        _infoItemHideDuration
    );
    
    /// <summary>
    /// 收起武器面板动画
    /// </summary>
    private Tweener HideWeaponItem=>DOTween.To(value =>
        {
            _weaponSelectItem.rectTransform.offsetMax =
                new Vector2(_weaponSelectItem.rectTransform.offsetMax.x, value);
        },
        _weaponSelectItem.rectTransform.offsetMax.y,
        -_weaponInfoItemStartTop,
        _weaponInfoItemShowDuration
    );
    
    /// <summary>
    /// 隐藏角色信息动画
    /// </summary>
    private Tweener HideInfoItem=> DOTween.To(value =>
        {
            _infoItemImg.rectTransform.offsetMax =
                new Vector2(_infoItemImg.rectTransform.offsetMax.x, value);
        },
        _infoItemImg.rectTransform.offsetMax.y,
        _infoItemEndTop,
        _infoItemHideDuration
    );
}
