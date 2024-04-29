using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaringCheck : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player")
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();
            if (!controller.hasDead)
            {
                if (!GiantRockCrab.Instance.inRangePlayers.Contains(controller))
                {
                    GiantRockCrab.Instance.inRangePlayers.Add(controller);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();
            if (!GiantRockCrab.Instance.inRangePlayers.Contains(controller))
            {
                GiantRockCrab.Instance.inRangePlayers.Remove(other.gameObject.GetComponent<PlayerController>());
            }
        }
    }

}
