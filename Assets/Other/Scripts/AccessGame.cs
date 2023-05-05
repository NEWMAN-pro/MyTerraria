using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;
using Soultia.Voxel;

public class AccessGame : MonoBehaviour
{

    // 保存地图种子
    public void SaveRandomSeed()
    {
        AccessGameAll.data.randomSeed = GameManager.randomSeed;
    }

    // 保存玩家信息
    public void SavePlayerSate()
    {
        PlayState play = GameObject.Find("Player").GetComponent<PlayState>();
        AccessGameAll.data.playerName = play.playName;
        AccessGameAll.data.maxHP = play.maxHP;
        AccessGameAll.data.maxMP = play.maxMP;
    }

    // 保存玩家背包
    public void SaveBackpack()
    {
        AccessGameAll.data.items = Backpack.items;
    }

    // 保存宝箱队列
    public void SaveBox()
    {
        AccessGameAll.data.boxs = BoxList.boxs;
        AccessGameAll.data.boxsName = BoxList.boxsName;
    }

    // 保存世界信息
    public void SaveMap()
    {
        Dictionary<Vector3i, GameObject> chunks = Map.instance.chunks;
        foreach (var pair in chunks)
        {
            AccessGameAll.data.map[pair.Key] = chunks[pair.Key].GetComponent<Chunk>().blocks;
        }
    }
}
