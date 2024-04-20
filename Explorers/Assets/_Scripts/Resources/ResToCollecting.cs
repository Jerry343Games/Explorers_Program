using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResToCollecting : MonoBehaviour
{


    private ResourceType _resType;

    public void Init(ResourceType type)
    {
        _resType = type;
    }

    public void Collecting()
    {
        SceneManager.Instance.resTasks.Find(x => x.type == _resType).taskUI.GetComponent<UIResPanel>().currentNum++;
        //ÒôĞ§ÌØĞ§
        Destroy(gameObject);
    }


}
