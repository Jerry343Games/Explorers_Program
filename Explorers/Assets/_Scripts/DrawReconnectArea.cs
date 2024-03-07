using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawReconnectArea : MonoBehaviour
{
    private BoxCollider coll;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, coll.size);
    }

}
