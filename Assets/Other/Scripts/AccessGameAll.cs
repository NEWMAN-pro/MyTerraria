using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

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
}


public class AccessGameAll : MonoBehaviour
{
    public GameData data = new();
    // 玩家背包
    public Dictionary<byte, Item> items;
    // 宝箱队列
    public Dictionary<string, Dictionary<byte, Item>> boxs;
    // 宝箱名字队列
    public Dictionary<string, string> boxsName;
    // 基础路径
    public string path;
    // 背包字典路径
    public string backPath;
    // 宝箱字典路径
    public string boxPath;
    // 宝箱名字路径
    public string boxNamePath;

    private void Awake()
    {
        path = Application.dataPath + "/Save/GameData.txt";
        backPath = Application.dataPath + "/Save/Items.txt";
        boxPath = Application.dataPath + "/Save/Boxs.txt";
        boxNamePath = Application.dataPath + "/Save/BoxsName.txt";
        Directory.CreateDirectory(Application.dataPath + "/Save");
    }

    // 保存游戏
    public void SaveGame()
    {
        // 存入基础文件
        string json = JsonUtility.ToJson(data);
        StreamWriter writer = new(path, false);
        writer.Write(json);
        writer.Close();

        // 存入背包文件
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(backPath, FileMode.Create);
        formatter.Serialize(stream, items);
        stream.Close();

        // 存入宝箱文件
        stream = new FileStream(boxPath, FileMode.Create);
        formatter.Serialize(stream, boxs);
        stream.Close();

        // 存入宝箱名字文件
        stream = new FileStream(boxNamePath, FileMode.Create);
        formatter.Serialize(stream, boxsName);
        stream.Close();
    }

    // 读取游戏
    public void ReadGame()
    {
        // 读取基础文件
        StreamReader reader = new(path);
        string json = reader.ReadToEnd();
        reader.Close();
        data = JsonUtility.FromJson<GameData>(json);

        // 读取背包文件
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(backPath, FileMode.Open);
        items = (Dictionary<byte, Item>)formatter.Deserialize(stream);
        stream.Close();

        // 读取宝箱文件
        stream = new FileStream(boxPath, FileMode.Open);
        boxs = (Dictionary<string, Dictionary<byte, Item>>)formatter.Deserialize(stream);
        stream.Close();

        // 读取宝箱名字文件
        stream = new FileStream(boxNamePath, FileMode.Open);
        boxsName = (Dictionary<string, string>)formatter.Deserialize(stream);
        stream.Close();
    }
}
