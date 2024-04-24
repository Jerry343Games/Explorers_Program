using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public PlayerController playerController; // 玩家健康组件的引用

    private Coroutine timeoutCoroutine;

    public float waitForSecond;

    void OnEnable()
    {
        playerController.OnShieldDamage += ResetShieldTimeout; // 订阅事件
        ResetShieldTimeout();
    }

    void OnDisable()
    {
        playerController.OnShieldDamage -= ResetShieldTimeout; // 取消订阅
    }

    private void ResetShieldTimeout()
    {
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
        }
        timeoutCoroutine = StartCoroutine(DeactivateShieldAfterDelay(waitForSecond));
    }

    private IEnumerator DeactivateShieldAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyShield();
    }

    private void DestroyShield()
    {
        Destroy(gameObject); // 销毁护盾对象或执行其他清理操作
    }
}
