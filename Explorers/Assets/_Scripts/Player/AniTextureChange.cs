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
    public FrameTextureData[] texturesData;
    public int totalFrames;
    private Animator animator;
    private Material material;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (GetComponent<SpriteRenderer>() != null)
        {
            material = GetComponent<SpriteRenderer>().material;
        }

        // 注册动画事件监听
        animator.GetBehaviour<AnimationEventsListener>().OnSpriteChange += HandleSpriteChange;
    }

    void HandleSpriteChange(int frameIndex)
    {
        // 根据传入的frameIndex查找对应的贴图并更新材质
        foreach (var data in texturesData)
        {
            if (data.frameIndex == frameIndex)
            {
                if (material != null)
                {
                    material.SetTexture("_EmissionMap", data.emissionTexture);
                    material.SetTexture("_BumpMap", data.normalTexture);
                }
                break;
            }
        }
    }

    void OnDestroy()
    {
        // 移除事件监听，防止内存泄漏
        if (animator != null && animator.GetBehaviour<AnimationEventsListener>() != null)
        {
            animator.GetBehaviour<AnimationEventsListener>().OnSpriteChange -= HandleSpriteChange;
        }
    }
}
