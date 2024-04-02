using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInstance : MonoBehaviour
{
    public GameObject[] objectsToPlace; // 存储要随机放置的物体
    public List<Transform> targetPositions = new List<Transform>(); // 使用列表存储目标位置

    void Start()
    {
        PlaceObjectsRandomly();
    }
    
    void PlaceObjectsRandomly()
    {
        if (objectsToPlace.Length != targetPositions.Count)
        {
            Debug.LogError("The number of objects to place must match the number of target positions.");
            return;
        }

        for (int i = 0; i < objectsToPlace.Length; i++)
        {
            int randomIndex = Random.Range(0, targetPositions.Count); // 随机选择一个目标位置
            objectsToPlace[i].transform.position = targetPositions[randomIndex].position; // 将物体放置在随机选择的位置上
            targetPositions.RemoveAt(randomIndex); // 移除已使用的位置
        }
    }
}
