using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int randomSeed;

    void Awake()
    {
        //��Ĭ�ϵ����������Ϊ��ǰ��ʱ���
        TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        randomSeed = (int)timeSpan.TotalSeconds;
    }
}