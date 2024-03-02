using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人需要的一些公用的东西先放这
/// </summary>
public class EnemyManager : SingletonPersistent<EnemyManager>
{

    public GameObject[] players;
    private void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");



    }






}
