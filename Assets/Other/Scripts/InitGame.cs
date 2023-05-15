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
            // 否则加载存档
            AccessGameAll.ReadGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
