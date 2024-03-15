using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryByLifeTime : MonoBehaviour
{
    public float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestoryMyself",lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestoryMyself()
    {
        Destroy(gameObject);
    }
}
