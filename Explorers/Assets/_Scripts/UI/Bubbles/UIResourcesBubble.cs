using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem.iOS;

public class UIResourcesBubble : UIBubbleItem
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        SetPosition(gameObject1,gameObject2);
    }
    
}
