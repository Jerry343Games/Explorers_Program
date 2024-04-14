using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
/// <summary>
/// 敌人需要的一些公用的东西先放这
/// </summary>
/// 
[System.Serializable]
public struct EnemySpwanGroup
{
    public GameObject enemyGroup;
    public float spwanWeight;

}

public class EnemyManager : SingletonPersistent<EnemyManager>
{ 
    public GameObject battery;
    public List<EnemySpawner> spawners;
    //public List<GameObject> enemisToSpawn;
    public List<EnemySpwanGroup> enemySpwanGroups;
    public float spwanerDistanceToBattery = 2f;
    public int maxEnemisCount = 40;
    private bool canSpwanEnemy = true;
    public List<Transform> spwanersNearToFar;
    private void Start()
    {
        InvokeRepeating("CheckEnemySpwan", 0, 1f);
    }
    
    public void CheckEnemySpwan()
    {
        Debug.Log(GameObject.FindGameObjectsWithTag("Enemy").Length);
        if (GameObject.FindGameObjectsWithTag("Enemy").Length > maxEnemisCount) canSpwanEnemy = false;
        else canSpwanEnemy = true;
        if (!canSpwanEnemy || spawners == null || spawners.Count == 0 || battery == null) return;
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "SelectScene" && spawners.Count != 0)
            SpawnEnemy();
    }
    public void SpawnEnemy()
    {

        //for(int i = 0; i < spawners.Count; i++)
        //{
        /*if (spawners[i].isAlwaysSpawn)
        {
            spawners[i].SpawnAlways(SelectRandomMonster());
        }
        else
        {*/
        //if (spawners[i].enemyPrefab == null)
        //{
        //   if (enemySpwanGroups.Count == 0) return;
        //   spawners[i].enemyPrefab = SelectRandomMonster();

        // } 
        //  spawners[i].SpwanOnce();

        //}


        // }
        spwanersNearToFar = GetFilteredAndSortedGenerators(spwanerDistanceToBattery);
        for (int i = 0; i < 2; i++)
        {
            if (i > spwanersNearToFar.Count - 1) break;
            spwanersNearToFar[i].GetComponent<EnemySpawner>().SpwanOnce(SelectRandomMonster());
        }
        

    }

    public GameObject SelectRandomMonster()
    {
        float totalWeight = 0;
        foreach (var monster in enemySpwanGroups)
        {
            totalWeight += monster.spwanWeight;
        }

        float randomNum = Random.Range(0f, totalWeight);

        float weightSum = 0;
        foreach (var monster in enemySpwanGroups)
        {
            weightSum += monster.spwanWeight;
            if (randomNum <= weightSum)
            {
                return monster.enemyGroup;
            }
        }

        // 如果出现意外情况，返回列表中的最后一个元素
        return enemySpwanGroups[enemySpwanGroups.Count - 1].enemyGroup;
    }
    //获得距离电池一定距离外的怪物生成点由近到远的排序
    public List<Transform> GetFilteredAndSortedGenerators(float distance)
    {
        List<Transform> generatorsOutsideDistance = new List<Transform>();

        foreach (var generator in spawners)
        {
            if (generator.transform.position.y < battery.transform.position.y) // 只保留 generator 的 y 坐标小于 battery 的 y 坐标的生成点
            {
                float distanceToBattery = Vector2.Distance(generator.transform.position, battery.transform.position);
                if (distanceToBattery > distance)
                {
                    generatorsOutsideDistance.Add(generator.transform);
                }
            }
        }

        // 按距离由近到远排序
        generatorsOutsideDistance.Sort((gen1, gen2) =>
            Vector3.Distance(gen1.position, battery.transform.position).CompareTo(Vector2.Distance(gen2.position, battery.transform.position))
        );

        return generatorsOutsideDistance;
    }


}
