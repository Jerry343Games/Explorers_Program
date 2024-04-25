using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FrameTextureData
{
    public Texture2D emissionTexture;
    public Texture2D normalTexture;
    public int frameIndex;
}

public class AniTextureChange : MonoBehaviour
{
    public FrameTextureData[] texturesData; // 存储每帧对应的贴图信息
    private Material material;              // 存储材质，用以更新贴图

    void Start()
    {
        // 获取SpriteRenderer组件的材质
        if (GetComponent<SpriteRenderer>() != null)
        {
            material = GetComponent<SpriteRenderer>().material;
        }
    }

    // Animation Event触发的方法
    public void OnSpriteChange(int frameIndex)
    {
        // 遍历texturesData，找到对应帧索引的贴图并更新材质
        foreach (var data in texturesData)
        {
            if (data.frameIndex == frameIndex)
            {
                // 设置自发光和法线贴图
                material.SetTexture("_EmissionMap", data.emissionTexture);
                material.SetTexture("_BumpMap", data.normalTexture);
                break;
            }
        }
    }
}
