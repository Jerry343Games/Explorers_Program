using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidCollision : MonoBehaviour
{
    public List<GameObject> hasHitedPlayers = new List<GameObject>();
    private void OnParticleCollision(GameObject other)
    {
        if ((other.gameObject.tag == "Player"|| other.gameObject.tag == "Battery") && !hasHitedPlayers.Contains(other))
        {
            Debug.Log(other.name + "±»¸¯Ê´");
            hasHitedPlayers.Add(other);
            other.gameObject.GetComponent<PlayerController>().TakeDamage(GiantRockCrab.Instance.acidDamage);
            other.gameObject.GetComponent<PlayerController>().DefenceDown(GiantRockCrab.Instance.acidCorrodeDuration, GiantRockCrab.Instance.acidCorrodeRate);
        }
    }
}
