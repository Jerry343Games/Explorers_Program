using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidCollision : MonoBehaviour
{
    public List<GameObject> hasHitedPlayers = new List<GameObject>();
    private void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.tag=="Player" && hasHitedPlayers.Contains(other))
        {
            hasHitedPlayers.Add(other);
            other.gameObject.GetComponent<PlayerController>().TakeDamage(GiantRockCrab.Instance.acidDamage);
            other.gameObject.GetComponent<PlayerController>().DefenceDown(5, 0.1f);
        }
    }
}
