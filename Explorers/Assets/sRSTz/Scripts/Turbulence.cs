using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbulence : MonoBehaviour
{
    public float forceMagnitude = 50.0f; // ���Ĵ�С
    public Vector3 forceDirection; // ���ķ���

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
                // ʩ��һ����������
                rb.AddForce(forceDirection * forceMagnitude, ForceMode.Force);
            }
        }
    }
}
