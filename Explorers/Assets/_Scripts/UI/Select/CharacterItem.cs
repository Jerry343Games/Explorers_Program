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

    //��װ�����ֹλ�á�����ʱ��
    private float _weaponInfoItemEndTop = 128f;
    private float _weaponInfoItemStartTop = 292f;
    private float _weaponInfoItemShowDuration = 0.3f;
    //��ɫ��Ϣ��ֹλ�á�����ʱ��
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

    [Header("ָʾ��")]
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
    /// �����ɫ���ĵ���¼�
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
            
            //������
            Sequence q = DOTween.Sequence();
            q.Append(HideInfoItem);
            q.Append(ShowWeaponItem);
            
            //���������һ�������豸
            playerIndex++;
            if (playerIndex < uiSelectPanel.sceneManager.maxPlayer)
            {
                playerEventCollection.transform.GetChild(playerIndex).gameObject.SetActive(true);
            }
        }
    }
    
    /// <summary>
    /// �ر�����ѡ��
    /// </summary>
    private void CloseWeaponInfoItem()
    {
        confirmLightImg.color = confirmedColor;
        //������
        Sequence q = DOTween.Sequence();
        q.Append(HideWeaponItem);
        q.Append(ShowInfoItem);
    }
    
    /// <summary>
    /// ��ʾ������嶯��
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
    /// ��ʾ��ɫ������嶯��
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
    /// ����������嶯��
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
    /// ���ؽ�ɫ��Ϣ����
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
