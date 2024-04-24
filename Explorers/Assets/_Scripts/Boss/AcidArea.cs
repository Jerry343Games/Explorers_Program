using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidArea : MonoBehaviour
{
    private float _range;
    private float _damage;
    public void Init(float acidRange,float acidDamage)
    {
        _range = acidRange;

        _damage = acidDamage;
    }

}
