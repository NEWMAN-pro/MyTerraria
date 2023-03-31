using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int randomSeed;

    void Awake()
    {
        if (StartUI.flag)
        {
            // 如果是新游戏，则随机生成种子
            //让默认的随机数种子为当前的时间戳
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            randomSeed = (int)timeSpan.TotalSeconds;
        }
        else
        {
            // 如果是继续游戏，则获得之前种子
            randomSeed = AccessGameAll.data.randomSeed;
        }
    }
}