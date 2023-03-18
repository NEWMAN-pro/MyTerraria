using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxList : MonoBehaviour
{
    public static Dictionary<string, Dictionary<byte, Item> > boxs = new Dictionary<string, Dictionary<byte, Item> >();
    public static Dictionary<string, string> boxsName = new();

    private void Awake()
    {
        if (!StartUI.flag)
        {
            // ����Ǽ�����Ϸ
            boxs = AccessGameAll.data.boxs;
            boxsName = AccessGameAll.data.boxsName;
        }
    }

    // ���ӱ���
    public static void AddBox(string key)
    {
        // ��ʼ��50���ո�
        Dictionary<byte, Item> items = new();
        for(byte i = 0; i < 50; i++)
        {
            items[i] = null;
        }
        boxs.Add(key, items);
        boxsName.Add(key, "����");
    }

    // ɾ������
    public static void DelectBox(string key)
    {
        boxs.Remove(key);
        boxsName.Remove(key);
    }

    // ��ȡ����
    public static Dictionary<byte, Item> GetBox(string key, out string name)
    {
        name = boxsName.ContainsKey(key) ? boxsName[key] : "";
        return boxs.ContainsKey(key) ? boxs[key] : null;
    }

    // �޸ı���
    public static void SetBox(string key, Dictionary<byte, Item> items, string name)
    {
        boxs[key] = items;
        boxsName[key] = name;
    }

    // �жϱ����Ƿ�Ϊ��
    public static bool GetBoxEmpty(string key)
    {
        string name;
        return GetBox(key, out name).Values.All(v => ReferenceEquals(v, null));
    }

    // ���ٽű�ʱ
    private void OnDestroy()
    {
        boxs.Clear();
        boxsName.Clear();
    }
}
