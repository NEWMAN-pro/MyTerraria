using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxList : MonoBehaviour
{
    public static Dictionary<string, Dictionary<byte, Item> > boxs = new Dictionary<string, Dictionary<byte, Item> >();

    // ���ӱ���
    public static void AddBox(string key)
    {
        boxs.Add(key, new Dictionary<byte, Item>());
    }

    // ɾ������
    public static void DelectBox(string key)
    {
        if(boxs[key].Count != 0)
        {
            Debug.Log("�ñ��䲻Ϊ��");
            return;
        }
        boxs.Remove(key);
    }

    // ��ȡ����
    public static Dictionary<byte, Item> GetBox(string key)
    {
        return boxs.ContainsKey(key) ? boxs[key] : null;
    }

    // �޸ı���
    public static void SetBox(string key, Dictionary<byte, Item> items)
    {
        boxs[key] = items;
    }
}