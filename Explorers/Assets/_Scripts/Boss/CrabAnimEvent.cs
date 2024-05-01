using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabAnimEvent : MonoBehaviour
{
    public Transform acidPoint;
    public void AwakeAnimEnd()
    {
        GiantRockCrab.Instance.StartPatrol();
    }

    public void SpitAcidAction()
    {

        GameObject acidArea = Instantiate(Resources.Load<GameObject>("Effect/AicdFlow"), acidPoint.position, Quaternion.Euler(0, 90, 0));
        if (transform.localScale.z > 0)
        {
            acidArea.transform.localScale = new Vector3(3, 3, 3);
        }
        else
        {
            acidArea.transform.localScale = new Vector3(3, 3, -3);

        }
        acidArea.transform.SetParent(transform);
        Destroy(acidArea, 3f);
    }
}
