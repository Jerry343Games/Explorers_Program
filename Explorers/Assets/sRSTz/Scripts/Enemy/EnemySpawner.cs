using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    public GameObject enemyPrefab=null;
    public float spawnTime = 3f;
    
    private float _spawnTimer = 0;
    public float spawnRadius = 5f;
    public bool isAlwaysSpawn = false;
    private void Awake()
    {
        if (gameObject.CompareTag("SpawnerInWall")) return;
        EnemyManager.Instance.spawners.Add(this);
    }
    /// <summary>
    /// 持续只生成一种
    /// </summary>
    public void SpawnAlways()
    {
        if (GetClosestPlayerDistance() < spawnRadius) return;
        if (enemyPrefab&&HasTimerArrived())
        {
            Instantiate(enemyPrefab);
        }

    }
    public void SpawnAlways(GameObject prefab)
    {
        if (GetClosestPlayerDistance() < spawnRadius) return;
        if (prefab && HasTimerArrived())
        {
            Instantiate(prefab);
        }
    }
    /// <summary>
    /// 只生成一次，生成完后置空
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public bool SpawnOnce(GameObject gameObject)
    {
        
       // if (GetClosestPlayerDistance() < spawnRadius || !HasTimerArrived()) return false;
       // else
       // {
            enemyPrefab = gameObject;
            Instantiate(enemyPrefab);
            enemyPrefab.transform.position = transform.position;
            enemyPrefab = null;
            return true;
       // }
    }
    public bool SpawnOnce(GameObject gameObject,bool isSleep)
    {
        enemyPrefab = gameObject;
        Instantiate(enemyPrefab);
        enemyPrefab.transform.position = transform.position;
        if (!isSleep)
        {
            Enemy[] enemies = enemyPrefab.GetComponentsInChildren<Enemy>();
            foreach(var enemy in enemies)
            {
                enemy.StartledFromSleep();
                enemy.ChangeDetectRadius(30f);
            }
        }
            
        enemyPrefab = null;
        return true;
    }
    /// <summary>
    /// 检查是否到达生成时间
    /// </summary>
    /// <returns></returns>
    private bool HasTimerArrived()
    {
        if (_spawnTimer >= spawnTime)
        {
            _spawnTimer = 0;
            return true;
        }
        else
        {
            _spawnTimer += Time.deltaTime;
            return false;
        }
    }
    public float GetClosestPlayerDistance()
    {

        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        if (PlayerManager.Instance.gamePlayers.Count == 0) return 0;
        foreach (var character in PlayerManager.Instance.gamePlayers)
        {
            //if (character.CompareTag("Enemy")) continue;
            float distanceToPlayer = Vector2.Distance(transform.position, character.transform.position);
            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestPlayer = character;
            }
            
        }


        return closestDistance;
    }
}
