using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
/// <summary>
/// 敌人需要的一些公用的东西先放这
/// </summary>
public class EnemyManager : SingletonPersistent<EnemyManager>
{

    public List<GameObject> players;
    public GameObject battery;
    public List<EnemySpawner> spawners;
    public List<GameObject> enemisToSpawn;
    protected override void Awake()
    {
        base.Awake();
        spawners.AddRange(FindObjectsOfType<EnemySpawner>());
    }

    private void Update()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        
        for(int i = 0; i < spawners.Count; i++)
        {
            if (spawners[i].isAlwaysSpawn)
            {
                spawners[i].SpawnAlways();
            }
            else
            {
                if (spawners[i].enemyPrefab == null)
                {
                    if (enemisToSpawn.Count == 0) return;
                    spawners[i].enemyPrefab = enemisToSpawn[0];
                    enemisToSpawn.RemoveAt(0);
                }
                spawners[i].SpwanOnce();
            }
            
            
        }
    }





}
