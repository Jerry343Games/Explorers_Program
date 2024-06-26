using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public PlayerController playerController; // 玩家引用
    public GameObject shieldEffect;
    private Coroutine timeoutCoroutine;

    public float waitForSecond;

    void OnEnable()
    {
        shieldEffect.transform.localScale = Vector3.zero;
        playerController = transform.parent.GetComponent<PlayerController>();
        playerController.OnShieldDamage += ResetShieldTimeout; // 订阅事件
        //ResetShieldTimeout();
    }

    void OnDisable()
    {
        playerController.OnShieldDamage -= ResetShieldTimeout; // 取消订阅
    }

    private void ResetShieldTimeout()
    {
        ActiveShield();
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

    public void ActiveShield()
    {
        shieldEffect.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.2f);
    }
    
    private void DestroyShield()
    {
        shieldEffect.transform.DOScale(Vector3.zero, 0.2f);
    }
}
