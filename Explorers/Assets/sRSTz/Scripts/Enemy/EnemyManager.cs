using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������Ҫ��һЩ���õĶ����ȷ���
/// </summary>
public class EnemyManager : SingletonPersistent<EnemyManager>
{

    public GameObject[] players;
    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");



    }






}
