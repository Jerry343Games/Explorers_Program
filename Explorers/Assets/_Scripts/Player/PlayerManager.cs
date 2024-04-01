using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum OptionalFeature
{
    Logistics,//��ػ����˹��� ����
    Charging,//��ػ����˹��� ����
    Salvo,//�����ͻ����˹��� ����
    DestroyTorpedoes,//�����ͻ����˹��� ��������
    Dash,//�����ͻ����˹��� ��ײ
    SonicWave,//�����ͻ����˹��� ������
    TranquilizerGun,//�����ͻ����˹��� ����ǹ
    FloatingFort,//�����ͻ����˹��� ������̨
}

[Serializable]
public class AnimationTextureMapping
{
    public CharacterAnimation animationName;
    public Texture2D texture;
}
public class PlayerManager : SingletonPersistent<PlayerManager>
{
    public List<GameObject> players = new List<GameObject>();

    //����ְҵ���� ֵ��ѡ��ļ���
    public Dictionary<int, OptionalFeature> playerFeaturesDic = new Dictionary<int, OptionalFeature>();

    //Ԥ���ط�������
    public List<AnimationTextureMapping> mappings = new List<AnimationTextureMapping>();
    
    //ͨ���������ҷ���
    public Texture2D GetTextureByAnimationName(CharacterAnimation animationName)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.animationName == animationName)
            {
                return mapping.texture;
            }
        }
        return null; // ����Ҳ�����Ӧ����ͼ���򷵻�null
    }

}
