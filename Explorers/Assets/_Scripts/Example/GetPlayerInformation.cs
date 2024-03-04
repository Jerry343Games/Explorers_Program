using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GetPlayerInformation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string name = other.name;
        int index = other.GetComponent<PlayerController>().myIndex;
        float batteryPower;
        
        int value = 10;
        other.GetComponent<Battery>().ChangePower(-value);
        batteryPower = other.GetComponent<Battery>().currentPower;
        Debug.Log("����"+name+"������"+value+"; ��ǰ������"+batteryPower);
        
    }
}
