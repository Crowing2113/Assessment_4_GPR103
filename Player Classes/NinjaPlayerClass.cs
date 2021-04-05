using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaPlayerClass : PlayerBaseClass
{
    // Start is called before the first frame update
    void Start()
    {
        SetHP(100);
        SetATK(10);
        SetDEF(5);
        SetSpeed(10);
        SetPlayerType("Ninja");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
