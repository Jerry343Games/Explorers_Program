using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public struct PropChance
{
    public GameObject propPrefab;
    public float chance; // 这是爆率，可以根据实际需求调整
}

public class PropChest : MonoBehaviour
{
    // 在Inspector中配置道具和概率列表，可以不同的箱子出装备的类型概率不同
    public List<PropChance> propChances;

    /// <summary>
    /// 开箱
    /// </summary>
    public void OpenChest()
    {
        //开箱动画

        GameObject selectedPropPrefab = ChooseRandomPropPrefab();
        if(selectedPropPrefab != null)
        {
            Instantiate(selectedPropPrefab, transform.position, Quaternion.identity);
        }

        //延迟消失
        Destroy(gameObject, .5f);
    }

    /// <summary>
    /// 由权重抽取
    /// </summary>
    /// <returns></returns>
    private GameObject ChooseRandomPropPrefab()
    {
        float totalChance = 0f;
        foreach (var propChance in propChances)
        {
            totalChance += propChance.chance;
        }

        float randomPoint = Random.Range(0, totalChance);
        foreach (var propChance in propChances)
        {
            if(randomPoint < propChance.chance)
            {
                return propChance.propPrefab;
            }
            randomPoint -= propChance.chance;
        }

        return null; // 没有选中任何道具
    }
}
