using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct ObjectChance
{
    public GameObject objectPrefab;
    public float chance; // ���Ǳ��ʣ����Ը���ʵ���������
}
public class RandomGenerator : MonoBehaviour
{
    // ��Inspector��������Ʒ�͸����б�
    public List<ObjectChance> objectChances;

    public List<Transform> spawnPoints;

    private List<bool> pointHasItem = new List<bool>();

    public int spawnCount;

    private void Awake()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            pointHasItem.Add(false);
        }
        GenerateObjectsInPoint();
    }
    public GameObject GetRandomObject()
    {
        if (objectChances.Count == 0)
        {
            return null;
        }

        float totalWeight = 0f;
        foreach (var objectChance in objectChances)
        {
            totalWeight += objectChance.chance;
        }

        if (totalWeight <= 0)
        {
            return null;
        }

        float randomPoint = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var objectChance in objectChances)
        {
            cumulativeWeight += objectChance.chance;
            if (randomPoint < cumulativeWeight)
            {
                return objectChance.objectPrefab;
            }
        }

        return null;
    }



    public void GenerateObjectsInPoint()
    {

        int i = 0;
        while(i< spawnCount)
        {
            int randomNum = Random.Range(0, spawnPoints.Count);
            if (!pointHasItem[randomNum])
            {
                GameObject obj = Instantiate(GetRandomObject(),
    spawnPoints[randomNum].position, Quaternion.identity);
                pointHasItem[randomNum] = true;
                i++;
            }
        }
    }



}
