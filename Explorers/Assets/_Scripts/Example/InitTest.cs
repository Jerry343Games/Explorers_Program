using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTest : MonoBehaviour
{
    private void Start()
    {
        SceneManager.Instance.FindBattery();
        SceneManager.Instance.GameInit();
    }
}
