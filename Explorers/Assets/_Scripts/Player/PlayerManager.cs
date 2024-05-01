using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

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
    public Texture2D NormalMap;
    public Texture2D EmissionMap;
}
public class   PlayerManager : SingletonPersistent<PlayerManager>
{
    public int maxPlayerCount;
    //���������б� �Ǹ�����
    public List<GameObject> players = new List<GameObject>();

    //����ְҵ���� ֵ��ѡ��ļ���
    public Dictionary<int, OptionalFeature> playerFeaturesDic = new Dictionary<int, OptionalFeature>();
    
    public bool hasMainBattary;
    //������Ҫ�ĵ����б� ����ҽ�ɫ������
    public List<GameObject> gamePlayers = new List<GameObject>();
    //Ԥ���ط�������
    public List<AnimationTextureMapping> mappings = new List<AnimationTextureMapping>();

    public List<PlayerInfo> allPlayerInfos = new List<PlayerInfo>();


    /// <summary>
    /// ͨ�����������ҷ�����ͼ
    /// </summary>
    /// <param name="animationName"></param>
    /// <returns></returns>
    public Texture2D GetNormalByAnimationName(CharacterAnimation animationName)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.animationName == animationName)
            {
                return mapping.NormalMap;
            }
        }
        return null; // ����Ҳ�����Ӧ����ͼ���򷵻�null
    }
    
    /// <summary>
    /// ͨ�����������ҷ�����ͼ
    /// </summary>
    /// <param name="animationName"></param>
    /// <returns></returns>
    public Texture2D GetEmissionByAnimationName(CharacterAnimation animationName)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.animationName == animationName)
            {
                return mapping.EmissionMap;
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

    public void AddPlayerInfo(PlayerInfo playerInfo)
    {
        allPlayerInfos.Add(playerInfo);
        //PrintAllPlayerInfos();
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
    
    /// <summary>
    /// ��ӡ�����Ϣ�б�
    /// </summary>
    public void PrintAllPlayerInfos()
    {
        foreach (PlayerInfo playerInfo in allPlayerInfos)
        {
            Debug.Log(playerInfo.ToString());
        }
    }

    
}

