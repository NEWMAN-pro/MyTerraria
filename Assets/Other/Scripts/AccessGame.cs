using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessGame : MonoBehaviour
{
    public AccessGameAll AGA;

    private void Awake()
    {
        AGA = GameObject.Find("Map").GetComponent<AccessGameAll>();
    }

    // 保存地图种子
    public void SaveRandomSeed()
    {
        AGA.data.randomSeed = GameManager.randomSeed;
    }

    // 保存玩家信息
    public void SavePlayerSate()
    {
        PlayState play = GameObject.Find("Player").GetComponent<PlayState>();
        AGA.data.playerName = play.playName;
        AGA.data.maxHP = play.maxHP;
        AGA.data.maxMP = play.maxMP;
    }

    // 保存玩家背包
    public void SaveBackpack()
    {
        AGA.items = this.GetComponent<Backpack>().items;
    }

    // 保存宝箱队列
    public void SaveBox()
    {
        AGA.boxs = BoxList.boxs;
        AGA.boxsName = BoxList.boxsName;
    }

    // 读取地图种子
    public void ReadRandomSeed()
    {
        GameManager.randomSeed = AGA.data.randomSeed;
    }

    // 读取玩家信息
    public void ReadPlayerSate()
    {
        PlayState play = GameObject.Find("Player").GetComponent<PlayState>();
        play.playName = AGA.data.playerName;
        play.maxHP = AGA.data.maxHP;
        play.maxMP = AGA.data.maxMP;
    }

    // 读取玩家背包
    public void ReadBackpack()
    {
        Backpack backpack = this.GetComponent<Backpack>();
        for(byte i = 0; i < AGA.items.Count; i++)
        {
            var pair = AGA.items[i];
            backpack.SetItem(i, pair);
        }
    }

    // 读取宝箱队列
    public void ReadBox()
    {
        foreach(var pair in AGA.boxs)
        {
            BoxList.SetBox(pair.Key, pair.Value, AGA.boxsName[pair.Key]);
        }
    }
}
