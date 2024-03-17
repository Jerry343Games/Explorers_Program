using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UISelectPanel : MonoBehaviour
{
    public Button battaryBtn;
    public Button shooterBtn;
    public Button fighterBtn;
    public Button healerBtn;

    public MultiplayerEventSystem battaryEventSystem;
    public MultiplayerEventSystem shooterEventSystem;
    public MultiplayerEventSystem fighterEventSystem;
    public MultiplayerEventSystem healerEventSystem;
    
    void Start()
    {
        battaryBtn.onClick.AddListener(ClickBattary);
        shooterBtn.onClick.AddListener(ClickShooter);
        fighterBtn.onClick.AddListener(ClickFighter);
        healerBtn.onClick.AddListener(ClickHealer);
    }

    private void ClickBattary()
    {
        battaryEventSystem = MultiplayerEventSystem.current.gameObject.GetComponent<MultiplayerEventSystem>();
        Debug.Log(battaryEventSystem.name);
    }

    private void ClickShooter()
    {
        shooterEventSystem = MultiplayerEventSystem.current.gameObject.GetComponent<MultiplayerEventSystem>();
        Debug.Log(shooterEventSystem.name);
    }

    private void ClickFighter()
    {
        
    }

    private void ClickHealer()
    {
        
    }
}
