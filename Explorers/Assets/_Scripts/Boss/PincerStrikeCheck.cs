using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PincerStrikeCheck : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player" || other.gameObject.tag == "Battery")
        {
            Debug.Log("ЧЏЛїУќжа");
            other.gameObject.GetComponent<PlayerController>().TakeDamage(GiantRockCrab.Instance.strikeDamage);
            other.gameObject.GetComponent<PlayerController>().Vertigo(
                new Vector3(Random.Range(-1, 1) * GiantRockCrab.Instance.strikeForce, 0/*Random.Range(-1, 1) * GiantRockCrab.Instance.strikeForce*/, 0),
                ForceMode.Impulse, 2f);
        }
    }


}
