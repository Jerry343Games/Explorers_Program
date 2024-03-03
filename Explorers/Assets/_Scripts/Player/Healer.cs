using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : PlayerController
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerInit();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterMove();
    }
}
