using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxList : MonoBehaviour
{
    public static Dictionary<string, Dictionary<byte, Item> > boxs = new Dictionary<string, Dictionary<byte, Item> >();
    public static Dictionary<string, string> boxsName = new();

    // Ôö¼Ó±¦Ïä
    public static void AddBox(string key)
    {
        // ³õÊ¼»¯50¸ö¿Õ¸ñ
        Dictionary<byte, Item> items = new();
        for(byte i = 0; i < 50; i++)
        {
            items[i] = null;
        }
        boxs.Add(key, items);
        boxsName.Add(key, "±¦Ïä");
    }

    // É¾³ý±¦Ïä
    public static void DelectBox(string key)
    {
        boxs.Remove(key);
        boxsName.Remove(key);
    }

    // »ñÈ¡±¦Ïä
    public static Dictionary<byte, Item> GetBox(string key, out string name)
    {
        name = boxsName.ContainsKey(key) ? boxsName[key] : "";
        return boxs.ContainsKey(key) ? boxs[key] : null;
    }

    // ÐÞ¸Ä±¦Ïä
    public static void SetBox(string key, Dictionary<byte, Item> items, string name)
    {
        boxs[key] = items;
        boxsName[key] = name;
    }

    // ÅÐ¶Ï±¦ÏäÊÇ·ñÎª¿Õ
    public static bool GetBoxEmpty(string key)
    {
        string name;
        return GetBox(key, out name).Values.All(v => ReferenceEquals(v, null));
    }
}
