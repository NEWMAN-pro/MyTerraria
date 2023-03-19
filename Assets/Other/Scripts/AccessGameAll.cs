using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Soultia.Util;

[System.Serializable]
public class GameData
{
    // ��ͼ����
    public int randomSeed;
    // �������
    public string playerName;
    // ����������ֵ
    public int maxHP;
    // ����������
    public int maxMP;
    // ��ұ���
    public Dictionary<byte, Item> items;
    // �������
    public Dictionary<string, Dictionary<byte, Item>> boxs;
    // �������ֶ���
    public Dictionary<string, string> boxsName;
    // ��ͼ��Ϣ
    public Dictionary<Vector3i, byte[,,]> map = new();
}


public class AccessGameAll : MonoBehaviour
{
    public static GameData data = new();
    // ����·��
    public static string path;

    private void Awake()
    {
        path = Application.dataPath + "/Save/";
        Directory.CreateDirectory(Application.dataPath + "/Save");
    }

    // ������Ϸ
    public static void SaveGame()
    {
        Debug.Log(path + StartUI.key);
        // ��������
        BinaryFormatter formatter = new();
        FileStream stream = new(path + StartUI.key, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    // ��ȡ��Ϸ
    public static GameData ReadGame()
    {
        Debug.Log(path + StartUI.key);
        // ��ȡ����
        BinaryFormatter formatter = new();
        FileStream stream = new(path + StartUI.key, FileMode.Open);
        data = (GameData)formatter.Deserialize(stream);
        stream.Close();
        Debug.Log(data.playerName);
        return data;
    }
}
