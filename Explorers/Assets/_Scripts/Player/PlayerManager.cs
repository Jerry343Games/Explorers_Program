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
    public int maxPlayerCount;
    //���������б� �Ǹ�����
    public List<GameObject> players = new List<GameObject>();

    //����ְҵ���� ֵ��ѡ��ļ���
    public Dictionary<int, OptionalFeature> playerFeaturesDic = new Dictionary<int, OptionalFeature>();

    //Ԥ���ط�������
    public List<AnimationTextureMapping> mappings = new List<AnimationTextureMapping>();

    public bool hasMainBattary;
    //������Ҫ�ĵ����б� ����ҽ�ɫ������
    public List<GameObject> gamePlayers = new List<GameObject>();

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

    /// <summary>
    /// ע�����
    /// </summary>
    /// <param name="player"></param>
    public void RegisterPlayer(GameObject player)
    {
        Debug.Log("Register Player");
        players.Add(player);
    }

    /// <summary>
    /// ������һ��ע������
    /// </summary>
    /// <returns></returns>
    public GameObject GetLatestPlayer()
    {
        if (players.Count > 0)
        {
            Debug.Log("GetLastPlayer");
            return players[players.Count - 1];
        }

        return null;
    }

    /// <summary>
    /// �����������
    /// </summary>
    public void DestoryAllPlayers()
    {
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
        players.Clear();
    }

}
