using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excavator : MonoBehaviour
{
    public float diggingSpeed;

    private float _diggingTime;

    private float _diggingTimer;

    private Resource _res;

    public void Init(Resource res,float diggingDuration)
    {
        _res = res;

        _diggingTime = diggingDuration;

        _diggingTimer = _diggingTime;
    }

    void Update()
    {
        if(_diggingTimer>0)
        {
            _diggingTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
