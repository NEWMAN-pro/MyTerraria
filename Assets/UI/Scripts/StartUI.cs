﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    // 存档名字
    public static string key = "";
    // 角色名字
    public static string playerName = "";
    // 地图种子
    public static int seed = -1;
    // 是否是新游戏
    public static bool flag = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 跳转页面
    public static void ToScenes()
    {
        SceneManager.LoadScene(1);
    }

    // 新游戏
    public void NewGame()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        this.transform.GetChild(3).gameObject.SetActive(true);
    }

    // 继续游戏
    public void Continue()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        this.transform.GetChild(2).gameObject.SetActive(true);
    }

    // 退出游戏
    public void ExitGameButton()
    {
        Application.Quit();
    }

    // 创建世界
    public void CreateWorld()
    {
        playerName = GameObject.Find("InputPlayerName").GetComponent<InputField>().text;
        key = GameObject.Find("InputWorldName").GetComponent<InputField>().text + ".txt";
        seed = int.Parse(GameObject.Find("Seed").GetComponent<InputField>().text);
        flag = true;
        ToScenes();
    }

    // 返回主界面
    public void Back()
    {
        this.transform.GetChild(1).gameObject.SetActive(true);
        this.transform.GetChild(2).gameObject.SetActive(false);
        this.transform.GetChild(3).gameObject.SetActive(false);
        this.transform.GetChild(3).GetChild(1).gameObject.GetComponent<InputField>().text = "";
        this.transform.GetChild(3).GetChild(3).gameObject.GetComponent<InputField>().text = "";
    }
}
