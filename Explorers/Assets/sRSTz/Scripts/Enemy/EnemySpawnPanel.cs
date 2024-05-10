using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnPanel : MonoBehaviour
{
    public RectTransform warning;
    public RectTransform clockImg;
    public RectTransform banner;
    public TMP_Text countdownText;
    private bool isCounting = false;
    public GameObject warningFinalPoint;
    private Vector2 warningFinalPosition;
    private void Awake()
    {
        EnemyManager.Instance.spawnPanel = this;
        warningFinalPosition = warningFinalPoint.transform.position;
    }
    public void StartCountdown(float time)
    {

        if (!isCounting)
        {
            Sequence q = DOTween.Sequence();
            q.Append(warning.DOAnchorPos(new Vector2(2,-66), 0.5f));
            q.Append(clockImg.DOAnchorPos(new Vector2(-109.6f, -57f), 0.25f));
            q.Append(banner.DOAnchorPos(new Vector2(-125f, -0), 0.25f)).OnComplete(()=>StartCountdownText(time));
            isCounting = true;
        }
    }

    /// <summary>
    /// 走表
    /// </summary>
    /// <param name="time"></param>
    private void StartCountdownText(float time)
    {
        DOTween.To(() => time, x => time = x, 0f, time)
            .OnUpdate(() => {
                countdownText.text = time.ToString("F0");
            })
            .OnComplete(() => {
                countdownText.text = "0";
                HideWarning();
                // 倒计时结束后执行的操作
                EnemyManager.Instance.SpawnEnemyAfter();
                //MusicManager.Instance.PlayBackMusic("Select");

            });
    }

    /// <summary>
    /// 隐藏动画
    /// </summary>
    private void HideWarning()
    {
        Sequence q = DOTween.Sequence();
        q.Append(banner.DOAnchorPos(new Vector2(-341f, -0), 0.25f));
        q.Append(clockImg.DOAnchorPos(new Vector2(-2.8f, -57f), 0.25f));
        q.Append(warning.DOAnchorPos(new Vector2(0, 273f), 0.5f)).OnComplete(()=>
        {
            countdownText.text = "!";
            isCounting = false;
        });
    }

    // private IEnumerator CountdownCoroutine(float timeLeft)
    // {
    //     
    //     countdownText.text = timeLeft.ToString();
    //
    //     while (timeLeft > 0)
    //     {
    //         yield return new WaitForSeconds(1.0f);
    //         timeLeft--;
    //         countdownText.text = "距离大量敌人到达："+timeLeft.ToString();
    //     }
    //
    //     countdownText.gameObject.SetActive(false); // 关闭text
    //     isCounting = false;
    //     EnemyManager.Instance.SpawnEnemyAfter();
    // }
}
