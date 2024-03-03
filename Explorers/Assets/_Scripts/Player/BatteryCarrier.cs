using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryCarrier : PlayerController
{
    void Start()
    {
        PlayerInit();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterMove();
    }
}
