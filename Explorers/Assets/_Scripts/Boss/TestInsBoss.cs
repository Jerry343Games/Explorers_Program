using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInsBoss : MonoBehaviour
{

    public Transform bossInsTran;
    public GameObject bossPrefab;

    private void Start()
    {
        if(SceneManager.Instance.isSecondLevel)
        {
            GameObject.FindGameObjectWithTag("Portal").transform.GetChild(0).gameObject.SetActive(true);//����Ǳͧ
        }
        else
        {
            GameObject.FindGameObjectWithTag("Portal").transform.GetChild(1).gameObject.SetActive(true);//������Ϸ��

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.tag == "Player" || other.gameObject.tag == "Battery") && SceneManager.Instance.isSecondLevel)
        {
            Instantiate(bossPrefab, bossInsTran.position, Quaternion.identity);
            MusicManager.Instance.PlayBackMusic("Boss");
            GetComponent<SphereCollider>().enabled = false;
        }
    }
}
