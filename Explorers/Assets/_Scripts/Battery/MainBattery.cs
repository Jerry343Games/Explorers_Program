using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBattery : Battery
{

    private bool gameOver;
    public MainBattery(int initialPower) : base(initialPower)
    {
    }
    private void Awake()
    {
        currentPower = maxPower;
        
        //����ÿ�����˥��
        InvokeRepeating("PowerDecayPreSecond", 1f, 1f);
    }

    private void Update()
    {
        if(currentPower<=0 && !GetComponent<PlayerController>().hasDead)
        {
            foreach(var player in PlayerManager.Instance.gamePlayers)
            {
                player.GetComponent<Battery>().currentPower = 0;
                player.GetComponent<PlayerController>().SetDeadState(true);
            }

            //�и�
            if(!gameOver)
            {
                gameOver = true;
                SceneManager.Instance.GameOver(false);
            }

        }
    }

}
