using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        if(!StartUI.flag)
        {
            // ������ش浵
            AccessGameAll.ReadGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
