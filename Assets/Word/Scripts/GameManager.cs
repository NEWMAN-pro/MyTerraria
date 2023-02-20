using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int randomSeed;

    void Awake()
    {
        //让默认的随机数种子为当前的时间戳
        TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        randomSeed = (int)timeSpan.TotalSeconds;
    }
}