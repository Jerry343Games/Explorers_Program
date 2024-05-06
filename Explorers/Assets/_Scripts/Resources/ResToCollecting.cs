using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResToCollecting : MonoBehaviour
{


    private ResourceType _resType;

    public int collectingAmount = 10;

    public void Init(ResourceType type)
    {
        _resType = type;
    }

    public void Collecting()
    {
        SceneManager.Instance.resTasks.Find(x => x.type == _resType).taskUI.GetComponent<UIResPanel>().AddNum(collectingAmount,transform);
        PlayerManager.Instance.ChangeResData(collectingAmount);
        //??งน??งน
        Destroy(gameObject);
    }


}
