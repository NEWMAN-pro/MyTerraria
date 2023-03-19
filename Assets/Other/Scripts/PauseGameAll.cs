using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameAll : MonoBehaviour
{
    // 暂停脚本队列
    public List<PauseGame> pgList = new();

    // 添加暂停脚本
    public void Add(PauseGame pg)
    {
        pgList.Add(pg);
    }

    // 暂停所有脚本
    public void OnPauseGame()
    {
        //Time.timeScale = 0f;
        foreach(var pg in pgList)
        {
            pg.OnPauseGame();
        }
    }

    // 解除所有脚本暂停
    public void UnPauseGame()
    {
        //Time.timeScale = 1f;
        foreach (var pg in pgList)
        {
            pg.UnPauseGame();
        }
    }
}
