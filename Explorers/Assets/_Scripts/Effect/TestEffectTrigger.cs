using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffectTrigger : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Aicd Hit");
    }
}
