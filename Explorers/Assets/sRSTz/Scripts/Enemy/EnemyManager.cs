using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
/// <summary>
/// 敌人需要的一些公用的东西先放这
/// </summary>
public class EnemyManager : SingletonPersistent<EnemyManager>
{ 
    public GameObject battery;
    public List<EnemySpawner> spawners;
    public List<GameObject> enemisToSpawn;
    protected override void Awake()
    {
        base.Awake();
        
    }

    private void Update()
    {
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "SelectScene"&&spawners.Count!=0)
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        
        for(int i = 0; i < spawners.Count; i++)
        {
            if (spawners[i].isAlwaysSpawn)
            {
                spawners[i].SpawnAlways(enemisToSpawn[Random.Range(0,enemisToSpawn.Count-1)]);
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
