﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : MonoBehaviour
{
    // 玩家出生点
    public Vector3 home = new(0, 25, 0);
    // 玩家名字
    public string playName = "";
    // 玩家血量
    public int HP = 100;
    // 最大血量
    public int maxHP = 100;
    // 玩家蓝量
    public int MP = 100;
    // 最大蓝量
    public int maxMP = 100;
    // 防御
    public int defenes = 0;
    // 伤害
    public int damage = 10;

    // 玩家状态条
    public State state;

    private void Awake()
    {
        if (!StartUI.flag)
        {
            // 如果是继续游戏
            GameData data = AccessGameAll.data;
            this.playName = data.playerName;
            this.maxHP = data.maxHP;
            this.maxMP = data.maxMP;
            this.HP = this.maxHP;
            this.MP = this.maxMP;
        }
        else
        {
            this.playName = StartUI.playerName;
        }
    }

    private void OnEnable()
    {
        HP = maxHP;
        MP = maxMP;
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameObject.Find("UI").transform.GetChild(6).GetComponent<State>();
        state.CreateUI(HP, maxHP, true);
        state.CreateUI(MP, maxMP, false);
        InvokeRepeating(nameof(RecoverMP), 0, 1);
    }

    private void Update()
    {
    }

    // 血量变化
    public void SetHP(int hp)
    {
        HP = (HP + hp) <= maxHP ? (HP + hp) : maxHP;
        if(HP >= 0) state.CreateUI(HP, maxHP, true);
        if(HP <= 0)
        {
            Debug.Log("玩家死亡");
            GameObject.Find("UI").GetComponent<UI>().Die(true);
            return;
        }
    }

    // 蓝量变化
    public bool SetMP(int mp)
    {
        if(MP + mp < 0)
        {
            return false;
        }
        MP = (MP + mp) <= maxMP ? (MP + mp) : maxMP;
        if (MP < 0) MP = 0;
        state.CreateUI(MP, maxMP, false);
        return true;
    }

    // 恢复蓝量
    public void RecoverMP()
    {
        int mp = Mathf.Max(1, (int)Mathf.Ceil(MP * 0.03f));
        SetMP(mp);
    }

    // 设置出生点
    public void SetHome(Vector3 posi)
    {
        this.home = posi;
    }

    // 复活
    public void Revive()
    {
        SetHP(100);
        SetMP(100);
        this.GetComponent<CharacterController>().enabled = false;
        this.transform.position = home;
        this.GetComponent<CharacterController>().enabled = true;
    }
}
