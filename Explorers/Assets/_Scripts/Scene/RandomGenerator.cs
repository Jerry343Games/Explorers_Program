using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;


[System.Serializable]
public struct ObjectChance
{
    public GameObject objectPrefab;
    public float chance; // 这是爆率，可以根据实际需求调整
}
public class RandomGenerator : MonoBehaviour
{
    // 在Inspector中配置物品和概率列表
    public List<ObjectChance> objectChances;

    public List<Transform> spawnPoints;

    public int spawnCount;

    private void Start()
    {
        GenerateObjectsInPoint();
    }

    public GameObject GetRandomObject()
    {
        if (objectChances.Count == 0)
        {
            Debug.LogWarning("物品列表为空，无法生成物体");
            return null;
        }

        float totalWeight = 0f;
        foreach (var objectChance in objectChances)
        {
            totalWeight += objectChance.chance;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("总权重为0，无法生成物体");
            return null;
        }

        float randomPoint = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var objectChance in objectChances)
        {
            cumulativeWeight += objectChance.chance;
            if (randomPoint < cumulativeWeight)
            {
                Debug.Log(1);
                return objectChance.objectPrefab;
            }
        }

        Debug.LogWarning("无法选择合适的物体");
        return null;
    }



    public void GenerateObjectsInPoint()
    {
        for(int i=0;i<spawnCount;i++)
        {
            GameObject obj = Instantiate(GetRandomObject(), 
                spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
        }
    }



}
