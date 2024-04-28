using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInsBoss : MonoBehaviour
{

    public Transform bossInsTran;
    public GameObject bossPrefab;

    private void Start()
    {
        Instantiate(bossPrefab, bossInsTran.position,Quaternion.Euler(0,90,0));
    }
}
