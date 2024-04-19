using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnPanel : MonoBehaviour
{
    public Text countdownText;
    private bool isCounting = false;
    private void Awake()
    {
        EnemyManager.Instance.spawnPanel = this;
    }
    public void StartCountdown(float time)
    {

        if (!isCounting)
        {
            countdownText.gameObject.SetActive(true);
            isCounting = true;
            StartCoroutine(CountdownCoroutine(time));
        }
    }

    private IEnumerator CountdownCoroutine(float timeLeft)
    {
        
        countdownText.text = timeLeft.ToString();

        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeLeft--;
            countdownText.text = "距离大量敌人到达："+timeLeft.ToString();
        }

        countdownText.gameObject.SetActive(false); // 关闭text
        isCounting = false;
        EnemyManager.Instance.SpawnEnemyAfter();
    }
}
