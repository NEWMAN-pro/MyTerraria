using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

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
}


public class AccessGameAll : MonoBehaviour
{
    public GameData data = new();
    // ��ұ���
    public Dictionary<byte, Item> items;
    // �������
    public Dictionary<string, Dictionary<byte, Item>> boxs;
    // �������ֶ���
    public Dictionary<string, string> boxsName;
    // ����·��
    public string path;
    // �����ֵ�·��
    public string backPath;
    // �����ֵ�·��
    public string boxPath;
    // ��������·��
    public string boxNamePath;

    private void Awake()
    {
        path = Application.dataPath + "/Save/GameData.txt";
        backPath = Application.dataPath + "/Save/Items.txt";
        boxPath = Application.dataPath + "/Save/Boxs.txt";
        boxNamePath = Application.dataPath + "/Save/BoxsName.txt";
        Directory.CreateDirectory(Application.dataPath + "/Save");
    }

    // ������Ϸ
    public void SaveGame()
    {
        // ��������ļ�
        string json = JsonUtility.ToJson(data);
        StreamWriter writer = new(path, false);
        writer.Write(json);
        writer.Close();

        // ���뱳���ļ�
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(backPath, FileMode.Create);
        formatter.Serialize(stream, items);
        stream.Close();

        // ���뱦���ļ�
        stream = new FileStream(boxPath, FileMode.Create);
        formatter.Serialize(stream, boxs);
        stream.Close();

        // ���뱦�������ļ�
        stream = new FileStream(boxNamePath, FileMode.Create);
        formatter.Serialize(stream, boxsName);
        stream.Close();
    }

    // ��ȡ��Ϸ
    public void ReadGame()
    {
        // ��ȡ�����ļ�
        StreamReader reader = new(path);
        string json = reader.ReadToEnd();
        reader.Close();
        data = JsonUtility.FromJson<GameData>(json);

        // ��ȡ�����ļ�
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(backPath, FileMode.Open);
        items = (Dictionary<byte, Item>)formatter.Deserialize(stream);
        stream.Close();

        // ��ȡ�����ļ�
        stream = new FileStream(boxPath, FileMode.Open);
        boxs = (Dictionary<string, Dictionary<byte, Item>>)formatter.Deserialize(stream);
        stream.Close();

        // ��ȡ���������ļ�
        stream = new FileStream(boxNamePath, FileMode.Open);
        boxsName = (Dictionary<string, string>)formatter.Deserialize(stream);
        stream.Close();
    }
}
