using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAnimEvent : MonoBehaviour
{

    public void AwakeAnimEnd()
    {
        GiantRockCrab.Instance.StartPatrol();
    }
}
