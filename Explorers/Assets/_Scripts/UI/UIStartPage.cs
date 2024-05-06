using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIStartPage : MonoBehaviour
{
    public LayerMask interactableLayer; // 设置一个Layer用于射线检测的过滤
    private GameObject lastHoveredObject; // 记录上一次鼠标停留的对象

    public Vector3 BtnStartScale = new Vector3(0.3722149f, 0.3722149f, 0.3722149f);
    public Vector3 BtnEndScale = new Vector3(0.39f, 0.39f, 0.39f);

    public Color defaultColor=new Color(1,1,1,1);
    public Color selectColor=new Color(1,1,1,1);
    
    public float intensity;

    private bool _isReady;
    [FormerlySerializedAs("_isOpenPanel")] public bool isOpenPanel;
    public Image maskImage;

    public GameObject defaultPanel;
    public GameObject playerNumPanel;

    public GameObject optionalPanel;

    private void Awake()
    {
        _isReady = false;
    }

    private void Start()
    {
        MusicManager.Instance.PlayBackMusic("Start");
        _isReady = false;
    }

    void Update()
    {
        if (!_isReady)
        {
            if (!isOpenPanel)
            {
                MouseActions();  
            }
        }
    }

    private void MouseActions()
    {
        // 检测鼠标移动到模型上的情况
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, interactableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // 如果鼠标停留在新的对象上
            if (lastHoveredObject != hitObject)
            {
                if (lastHoveredObject != null)
                {
                    // 对上一个对象执行移开鼠标的操作
                    OnMouseExit(lastHoveredObject);
                }

                // 对当前对象执行鼠标悬停的操作
                OnMouseEnter(hitObject);
                lastHoveredObject = hitObject;
            }

            // 如果点击了鼠标左键
            if (Input.GetMouseButtonDown(0))
            {
                OnClick(hitObject);
            }
        }
        else
        {
            if (lastHoveredObject != null)
            {
                // 当没有任何对象被鼠标悬停时执行
                OnMouseExit(lastHoveredObject);
                lastHoveredObject = null;
            }
        }
    }

    private void OnMouseEnter(GameObject gameObject)
    {
        MusicManager.Instance.PlaySound("开始界面悬停");

        // 这里添加鼠标悬停时的操作
        gameObject.transform.DOScale(BtnEndScale, 0.2f);
        float factor = Mathf.Pow(2, intensity);
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor",selectColor*factor);
    }

    private void OnMouseExit(GameObject gameObject)
    {
        // 这里添加鼠标移出时的操作
        gameObject.transform.DOScale(BtnStartScale, 0.2f);
        float factor = Mathf.Pow(2, intensity);
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor",defaultColor*factor);
        
    }

    private void OnClick(GameObject gameObject)
    {
        MusicManager.Instance.PlaySound("开始界面按钮");
        // 这里添加点击时的操作
        Sequence seq = DOTween.Sequence();
        seq.Append(gameObject.transform.DOScale(BtnEndScale + new Vector3(0.02f, 0.02f, 0.02f), 0.2f));
        seq.Append(gameObject.transform.DOScale(BtnEndScale, 0.2f));

        switch (gameObject.name)
        {
            case "start":
                defaultPanel.transform.DOLocalMove(new Vector3(0, -1.46f, 0), 0.5f)
                    .OnComplete(()=>
                    {
                        defaultPanel.SetActive(false);
                        playerNumPanel.SetActive(true);
                        playerNumPanel.transform.DOLocalMove(new Vector3(0.32f, -0.32f, 0.62f), 0.5f);
                    });
                break;
            case "options":
                optionalPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
                isOpenPanel = true;
                break;
            case "exit":
                Application.Quit();
                break;
            case "2PBtn":
                _isReady = true;
                seq.Append(maskImage.DOFade(1, 0.5f).OnComplete(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectScene");
                        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().maxPlayerCount = 2;
                    })
                );
                break;
            case "3PBtn":
                _isReady = true;
                seq.Append(maskImage.DOFade(1, 0.5f).OnComplete(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectScene");
                        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().maxPlayerCount = 3;
                    })
                );
                break;
            case "4PBtn":
                _isReady = true;
                seq.Append(maskImage.DOFade(1, 0.5f).OnComplete(() =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectScene");
                        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().maxPlayerCount = 4;
                    })
                );
                break;
        }
        
        if (gameObject.name=="exit")
        {
            
        }
        if (gameObject.name=="options")
        {
            
        }
        
    }
}
