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
        GameObject target = GiantRockCrab.Instance.FindNearestPlayer();
        GameObject acidArea = Instantiate(Resources.Load<GameObject>("Effect/AicdFlow"), acidPoint.position, Quaternion.Euler(0, 90, 0));
        
        if (target)
        {
            Vector3 dir = (target.transform.position - acidPoint.position).normalized;

            float angle = Vector3.Angle(Vector3.right, dir);

            if (dir.y >= 0)
            {
                acidArea.transform.rotation = Quaternion.Euler(-angle, 90, 0);
            }
            else
            {
                acidArea.transform.rotation = Quaternion.Euler(angle, 90, 0);

            }
        }
        else
        {
            if (transform.localScale.z > 0)
            {
                acidArea.transform.localScale = new Vector3(3, 3, 3);


            }
            else
            {
                acidArea.transform.localScale = new Vector3(3, 3, -3);
            }
        }
        Destroy(acidArea, 3f);
    }
}
