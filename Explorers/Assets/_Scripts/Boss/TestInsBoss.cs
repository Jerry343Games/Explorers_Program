using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInsBoss : MonoBehaviour
{

    public Transform bossInsTran;
    public GameObject bossPrefab;


    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.tag == "Player" || other.gameObject.tag == "Battery") && SceneManager.Instance.isSecondLevel)
        {
            Instantiate(bossPrefab, bossInsTran.position, Quaternion.identity);
            GetComponent<SphereCollider>().enabled = false;
        }
    }
}
