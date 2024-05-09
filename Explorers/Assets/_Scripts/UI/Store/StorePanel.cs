using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : MonoBehaviour
{
    public GameObject characterGrids;

    public Image maskShow;

    private Vector2 endPos = new Vector2(0, -15f);
    private Vector2 startPos = new Vector2(0, -439f);

    private int _confirmedPlayerNum;
    
    void Start()
    {
        characterGrids.GetComponent<RectTransform>().anchoredPosition = startPos;
        List<PlayerInfo> playerInfos = PlayerManager.Instance.allPlayerInfos;
        List<GameObject> players = PlayerManager.Instance.players;
        DisplayStore(players);
        
        
        //出场动画
        characterGrids.GetComponent<RectTransform>().DOAnchorPos(endPos, 0.5f);
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
            //7Chords
            characterBuffPanel.SetPlayerToItems();
            characterBuffPanel.Refresh();

            characterBuffPanel.OnConfirmClick += PlayerStateCheck;
            //个性化设置

        }
    }

    private void PlayerStateCheck()
    {
        _confirmedPlayerNum++;

        if (_confirmedPlayerNum==PlayerManager.Instance.maxPlayerCount)
        {
            Sequence q = DOTween.Sequence();
            q.AppendInterval(1f);
            q.Append(maskShow.DOFade(1,0.5f).OnComplete(()=>UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/JerryTest_2")));
        }
    }
}
