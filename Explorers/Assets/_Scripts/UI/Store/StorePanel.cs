using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePanel : MonoBehaviour
{
    public GameObject characterGrids; 
    
    void Start()
    {
        List<PlayerInfo> playerInfos = PlayerManager.Instance.allPlayerInfos;
        List<GameObject> players = PlayerManager.Instance.players;
        DisplayStore(players);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisplayStore(List<GameObject> players)
    {
        ShowGridsCount(players);
    }

    private void ShowGridsCount(List<GameObject> players)
    {
        int count = players.Count;
        foreach (GameObject player in players)
        {
            // 实例化预制体并设置为CharacterGrids的子对象
            GameObject item = Instantiate(Resources.Load<GameObject>("UI/Store/CharacterBuffPanel"),characterGrids.transform);
            CharacterBuffPanel characterBuffPanel = item.GetComponentInChildren<CharacterBuffPanel>();
            characterBuffPanel.player = player;
            characterBuffPanel.Refresh();
            
            //个性化设置
            
        }
    }
}
