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
    /// <summary>
    /// ����ֻ����һ��
    /// </summary>
    public void SpawnAlways()
    {
        if (GetClosestPlayerDistance() < spawnRadius) return;
        if (enemyPrefab&&HasTimerArrived())
        {
            Instantiate(enemyPrefab);
        }

    }
    /// <summary>
    /// ֻ����һ�Σ���������ÿ�
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public bool SpwanOnce()
    {

        if (GetClosestPlayerDistance() < spawnRadius || !HasTimerArrived()) return false;
        else
        {
            Debug.Log("22");
            Instantiate(enemyPrefab);
            
            enemyPrefab = null;
            return true;
        }
    }
    /// <summary>
    /// ����Ƿ񵽴�����ʱ��
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
        if (EnemyManager.Instance.players.Count == 0) return 0;
        foreach (var character in EnemyManager.Instance.players)
        {
            //if (character.CompareTag("Enemy")) continue;
            float distanceToPlayer = Vector3.Distance(transform.position, character.transform.position);
            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                closestPlayer = character;
            }
            
        }


        return closestDistance;
    }
}
