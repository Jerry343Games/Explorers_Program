using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInsBoss : MonoBehaviour
{

    public Transform bossInsTran;
    public GameObject bossPrefab;

    private void Start()
    {
        if(SceneManager.Instance.isSecondLevel)
        {
            GameObject.FindGameObjectWithTag("Portal").transform.GetChild(0).gameObject.SetActive(true);//����Ǳͧ
        }
        else
        {
            GameObject.FindGameObjectWithTag("Portal").transform.GetChild(1).gameObject.SetActive(true);//������Ϸ��

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.tag == "Player" || other.gameObject.tag == "Battery") && SceneManager.Instance.isSecondLevel)
        {
            CameraTrace.instance.CameraShake(1f, 2f);
            MusicManager.Instance.PlaySound("Boss��ʯ");
            MusicManager.Instance.PlaySound("Ǳͧ����");



            Instantiate(bossPrefab, bossInsTran.position, Quaternion.identity);
            MusicManager.Instance.PlayBackMusic("Boss");
            GetComponent<SphereCollider>().enabled = false;

            //Time.timeScale = 0;
            //Sequence s = DOTween.Sequence();
            //s.SetUpdate(UpdateType.Normal, true);
            //s.AppendInterval(0.2f).OnStart(()=> 
            //{
            //    //����ӳ�����UI��Player
            //    foreach (var player in PlayerManager.Instance.players)
            //    {
            //        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            //    }
            //    CameraTrace.instance.CameraShake(1f, 2f);
            //    MusicManager.Instance.PlaySound("Boss��ʯ");
            //    MusicManager.Instance.PlaySound("Ǳͧ����");

            //}).OnComplete(() =>
            //{
            //    Instantiate(bossPrefab, bossInsTran.position, Quaternion.identity);
            //    MusicManager.Instance.PlayBackMusic("Boss");
            //    GetComponent<SphereCollider>().enabled = false;
            //    Time.timeScale = 1;
            //    //����ӳ�����UI��Player
            //    foreach (var player in PlayerManager.Instance.players)
            //    {
            //        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
            //    }
            //});

            //Instantiate(bossPrefab, bossInsTran.position, Quaternion.identity);
            //MusicManager.Instance.PlayBackMusic("Boss");
            //GetComponent<SphereCollider>().enabled = false;
        }
    }
}
