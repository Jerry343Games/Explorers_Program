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

    public int spawnCount;

    private void Awake()
    {
        GenerateObjectsInPoint();
    }

    public GameObject GetRandomObject()
    {
        if (objectChances.Count == 0)
        {
            Debug.LogWarning("��Ʒ�б�Ϊ�գ��޷���������");
            return null;
        }

        float totalWeight = 0f;
        foreach (var objectChance in objectChances)
        {
            totalWeight += objectChance.chance;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("��Ȩ��Ϊ0���޷���������");
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

        Debug.LogWarning("�޷�ѡ����ʵ�����");
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
