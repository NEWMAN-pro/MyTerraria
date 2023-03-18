using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int randomSeed;

    void Awake()
    {
        if (StartUI.flag)
        {
            // ���������Ϸ���������������
            //��Ĭ�ϵ����������Ϊ��ǰ��ʱ���
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            randomSeed = (int)timeSpan.TotalSeconds;
        }
        else
        {
            // ����Ǽ�����Ϸ������֮ǰ����
            randomSeed = AccessGameAll.data.randomSeed;
        }
    }
}