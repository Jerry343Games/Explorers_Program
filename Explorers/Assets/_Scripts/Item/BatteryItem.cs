using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryItem : Item
{

    public int power;
    public override void Apply(GameObject user)
    {
        user.GetComponent<CellBattery>().ChangePower(power);
        Destroy(gameObject);
    }
}
