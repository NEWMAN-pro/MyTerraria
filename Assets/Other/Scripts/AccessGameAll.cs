using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Soultia.Util;

[System.Serializable]
public class GameData
{
    // 地图种子
    public int randomSeed;
    // 玩家名字
    public string playerName;
    // 玩家最大生命值
    public int maxHP;
    // 玩家最大蓝量
    public int maxMP;
    // 玩家背包
    public Dictionary<byte, Item> items;
    // 宝箱队列
    public Dictionary<string, Dictionary<byte, Item>> boxs;
    // 宝箱名字队列
    public Dictionary<string, string> boxsName;
    // 地图信息
    public Dictionary<Vector3i, byte[,,]> map = new();
}


public class AccessGameAll : MonoBehaviour
{
    public static GameData data = new();
    // 基础路径
    public static string path;

    private void Awake()
    {
        path = Application.dataPath + "/Save/";
        Directory.CreateDirectory(Application.dataPath + "/Save");
    }

    // 保存游戏
    public static void SaveGame()
    {
        Debug.Log(path + StartUI.key);
        // 保存数据
        BinaryFormatter formatter = new();
        FileStream stream = new(path + StartUI.key, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    // 读取游戏
    public static GameData ReadGame()
    {
        Debug.Log(path + StartUI.key);
        // 读取数据
        BinaryFormatter formatter = new();
        FileStream stream = new(path + StartUI.key, FileMode.Open);
        data = (GameData)formatter.Deserialize(stream);
        stream.Close();
        Debug.Log(data.playerName);
        return data;
    }
}
