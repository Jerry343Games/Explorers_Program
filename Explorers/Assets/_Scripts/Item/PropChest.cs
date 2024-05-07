using DG.Tweening;
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
        MusicManager.Instance.PlaySound("打开箱子");
        GetComponent<Collider>().enabled = false;
        //开箱动画
        transform.GetChild(0).GetComponent<Animator>().enabled = true;
        StartCoroutine(DelayToSpawnItem());
        //延迟消失
        Destroy(gameObject, 3.5f);
    }

    IEnumerator DelayToSpawnItem()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject selectedPropPrefab = ChooseRandomPropPrefab();
        if (selectedPropPrefab != null)
        {
            GameObject item = Instantiate(selectedPropPrefab, transform.position, Quaternion.identity);
            item.GetComponent<Collider>().enabled = false;
            Sequence s = DOTween.Sequence();

            s.Append(item.transform.DOMoveY(transform.position.y + 1, 0.5f));
            s.Append(item.transform.DOMoveY(transform.position.y, 0.5f).OnComplete(() =>
            {
                item.GetComponent<Collider>().enabled = true;
            }));


        }

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
