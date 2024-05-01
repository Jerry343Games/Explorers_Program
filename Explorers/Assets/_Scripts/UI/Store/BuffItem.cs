using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class BuffItem : MonoBehaviour
{
    [HideInInspector]
    public UpgradeBuff myBuff;

    public Image icon;
    public TMP_Text name;
    public TMP_Text description;

    private Button _myBtn;
    
    private void OnEnable()
    {
        _myBtn = GetComponent<Button>();
    }
    
    public void EnableRefresh()
    {
        name.text = myBuff.buffName;
        description.text = myBuff.description;
        icon.sprite = myBuff.buffIcon;
    }
}
