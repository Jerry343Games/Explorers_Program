using DG.Tweening;
using UnityEngine;

public class TestInsBoss : MonoBehaviour
{

    public Transform bossInsTran;
    public GameObject bossPrefab;

    private void Start()
    {
        if(SceneManager.Instance.isSecondLevel)
        {
            GameObject.FindGameObjectWithTag("Portal").transform.GetChild(0).gameObject.SetActive(true);//激活潜艇
        }
        else
        {
            GameObject.FindGameObjectWithTag("Portal").transform.GetChild(1).gameObject.SetActive(true);//激活游戏机

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.tag == "Player" || other.gameObject.tag == "Battery") && SceneManager.Instance.isSecondLevel)
        {
            Time.timeScale = 0;
            Sequence s = DOTween.Sequence();
            s.SetUpdate(UpdateType.Normal, true);
            s.AppendInterval(0.2f).OnStart(()=> 
            {
                CameraTrace.instance.CameraShake(1f, 2f);
                MusicManager.Instance.PlaySound("Boss飞石");
                MusicManager.Instance.PlaySound("潜艇警报");

            }).OnComplete(() =>
            {
                Time.timeScale = 1;
            });

            Instantiate(bossPrefab, bossInsTran.position, Quaternion.identity);
            MusicManager.Instance.PlayBackMusic("Boss");
            GetComponent<SphereCollider>().enabled = false;
        }
    }
}
