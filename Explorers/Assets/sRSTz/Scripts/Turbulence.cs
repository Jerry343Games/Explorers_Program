using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbulence : MonoBehaviour
{
    public float forceMagnitude = 50.0f; // 力的大小
    public Vector3 forceDirection; // 力的方向

    private void Awake()
    {
        forceDirection = transform.up;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Battery"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log(rb.name);
                // 施加一个持续的力
                rb.AddForce(forceDirection * forceMagnitude, ForceMode.Force);
            }
        }
    }
}
