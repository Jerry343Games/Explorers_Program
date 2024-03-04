using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SceneManager : MonoBehaviour
{
    public int maxPlayer=2;
    public bool isMaxPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMaxPlayer)
        {
            CountPlayer();
        }
    }

    private void CountPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length==maxPlayer)
        {
            isMaxPlayer = true;
            Debug.Log("MaxPlayer!:");
        }
    }
}
