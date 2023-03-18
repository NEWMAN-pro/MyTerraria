using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;
using Soultia.Voxel;

public class AccessGame : MonoBehaviour
{

    // �����ͼ����
    public void SaveRandomSeed()
    {
        AccessGameAll.data.randomSeed = GameManager.randomSeed;
    }

    // ���������Ϣ
    public void SavePlayerSate()
    {
        PlayState play = GameObject.Find("Player").GetComponent<PlayState>();
        AccessGameAll.data.playerName = play.playName;
        AccessGameAll.data.maxHP = play.maxHP;
        AccessGameAll.data.maxMP = play.maxMP;
    }

    // ������ұ���
    public void SaveBackpack()
    {
        AccessGameAll.data.items = this.GetComponent<Backpack>().items;
    }

    // ���汦�����
    public void SaveBox()
    {
        AccessGameAll.data.boxs = BoxList.boxs;
        AccessGameAll.data.boxsName = BoxList.boxsName;
    }

    // ����������Ϣ
    public void SaveMap()
    {
        Dictionary<Vector3i, GameObject> chunks = Map.instance.chunks;
        foreach (var pair in chunks)
        {
            AccessGameAll.data.map[pair.Key] = chunks[pair.Key].GetComponent<Chunk>().blocks;
        }
    }
}
