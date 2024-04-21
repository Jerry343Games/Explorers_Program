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
    public float enemySpwanTime = 180f;
    //[HideInInspector]
    public List<GameObject> turbulenceSpawners;//所有的湍流喷射，随机启用其中一部分
    
    public EnemySpawnPanel spawnPanel;
    private void Start()
    {
        InvokeRepeating("CheckEnemySpwan", 0, 1f);
        InvokeRepeating(nameof(UpdateEnemySpawnPanel), 0, enemySpwanTime);
    }
    
    public void CheckEnemySpwan()//开局刷怪和湍流
    {
        Debug.Log(GameObject.FindGameObjectsWithTag("Enemy").Length);
        if (GameObject.FindGameObjectsWithTag("Enemy").Length > maxEnemisCount) canSpwanEnemy = false;
        else canSpwanEnemy = true;
        if (!canSpwanEnemy || spawners == null || spawners.Count == 0 || battery == null) return;
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("SelectScene") && spawners.Count != 0)
        { SpawnEnemyStart(); TurbulenceStart(); CancelInvoke(nameof(CheckEnemySpwan)); }

    }
    //先刷新一波原生怪
    public void SpawnEnemyStart()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            
            foreach(var spwaner in spawners)
            {
               spwaner.SpawnOnce(SelectRandomMonster());
               
            }
        }


    }
    //刷一波虫潮 TODO：随机选同一边，然后再从这一边的角度随机选三个方向刷怪（暂定），直接刷墙里应该是可以的（注意把敌人与墙的碰撞取消）
    public void SpawnEnemyAfter()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("SelectScene")) return;
        spwanersNearToFar = GetFilteredAndSortedGeneratorsInWall (spwanerDistanceToBattery);
        for (int i = 0; i < 3; i++)
        {
            if (i > spwanersNearToFar.Count - 1) break;
            spwanersNearToFar[i].GetComponent<EnemySpawner>().SpawnOnce(SelectRandomMonster(),false);
            spwanersNearToFar[i].GetComponent<EnemySpawner>().SpawnOnce(SelectRandomMonster(), false);
            spwanersNearToFar[i].GetComponent<EnemySpawner>().SpawnOnce(SelectRandomMonster(), false);
        }
        Debug.Log("after" + GameObject.FindGameObjectsWithTag("Enemy").Length);
    }
    private void UpdateEnemySpawnPanel()
    {
        if (spawnPanel == null) return;
        spawnPanel.StartCountdown(10);
    }
    //启用一部分的湍流发射器
    public void TurbulenceStart()
    {
        if (turbulenceSpawners.Count != 0) return;
        turbulenceSpawners.AddRange(GameObject.FindGameObjectsWithTag("TurbulenceSpawner"));
        Debug.Log("12131" + turbulenceSpawners.Count);
        // 随机选择四分之一的turbulenceSpawner
        int quarterCount = Mathf.CeilToInt(turbulenceSpawners.Count / 2.5f);
        List<GameObject> selectedSpawners = new List<GameObject>();

        for (int i = 0; i < quarterCount; i++)
        {
            int randomIndex = Random.Range(0, turbulenceSpawners.Count);
            selectedSpawners.Add(turbulenceSpawners[randomIndex]);
            turbulenceSpawners.RemoveAt(randomIndex);
        }
        foreach(var spawner in turbulenceSpawners)
        {
            spawner.SetActive(false);
        }
        // 启用选中的spawners
        foreach (var spawner in selectedSpawners)
        {
            spawner.GetComponent<TurbulenceSpawner>().StartShoot();
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
    public List<Transform> GetFilteredAndSortedGeneratorsInWall(float distance)
    {
        List<Transform> generatorsOutsideDistance = new List<Transform>();
        List<GameObject> spawnersInWall=new();
        spawnersInWall.AddRange( GameObject.FindGameObjectsWithTag("SpawnerInWall"));
        foreach (var generator in spawnersInWall)
        {
            
                float distanceToBattery = Vector2.Distance(generator.transform.position, battery.transform.position);
                if (distanceToBattery > distance)
                {
                    generatorsOutsideDistance.Add(generator.transform);
                }
            
        }

        // 按距离由近到远排序
        generatorsOutsideDistance.Sort((gen1, gen2) =>
            Vector3.Distance(gen1.position, battery.transform.position).CompareTo(Vector2.Distance(gen2.position, battery.transform.position))
        );

        return generatorsOutsideDistance;
    }

}
