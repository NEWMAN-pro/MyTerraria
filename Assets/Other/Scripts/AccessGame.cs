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

    // �����ͼ����
    public void SaveRandomSeed()
    {
        AGA.data.randomSeed = GameManager.randomSeed;
    }

    // ���������Ϣ
    public void SavePlayerSate()
    {
        PlayState play = GameObject.Find("Player").GetComponent<PlayState>();
        AGA.data.playerName = play.playName;
        AGA.data.maxHP = play.maxHP;
        AGA.data.maxMP = play.maxMP;
    }

    // ������ұ���
    public void SaveBackpack()
    {
        AGA.items = this.GetComponent<Backpack>().items;
    }

    // ���汦�����
    public void SaveBox()
    {
        AGA.boxs = BoxList.boxs;
        AGA.boxsName = BoxList.boxsName;
    }

    // ��ȡ��ͼ����
    public void ReadRandomSeed()
    {
        GameManager.randomSeed = AGA.data.randomSeed;
    }

    // ��ȡ�����Ϣ
    public void ReadPlayerSate()
    {
        PlayState play = GameObject.Find("Player").GetComponent<PlayState>();
        play.playName = AGA.data.playerName;
        play.maxHP = AGA.data.maxHP;
        play.maxMP = AGA.data.maxMP;
    }

    // ��ȡ��ұ���
    public void ReadBackpack()
    {
        Backpack backpack = this.GetComponent<Backpack>();
        for(byte i = 0; i < AGA.items.Count; i++)
        {
            var pair = AGA.items[i];
            backpack.SetItem(i, pair);
        }
    }

    // ��ȡ�������
    public void ReadBox()
    {
        foreach(var pair in AGA.boxs)
        {
            BoxList.SetBox(pair.Key, pair.Value, AGA.boxsName[pair.Key]);
        }
    }
}
