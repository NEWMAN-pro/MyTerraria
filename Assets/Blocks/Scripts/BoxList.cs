using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxList : MonoBehaviour
{
    public static Dictionary<string, Dictionary<byte, Item> > boxs = new Dictionary<string, Dictionary<byte, Item> >();

    // 增加宝箱
    public static void AddBox(string key)
    {
        // 初始化50个空格
        Dictionary<byte, Item> items = new();
        for(byte i = 0; i < 50; i++)
        {
            items[i] = null;
        }
        boxs.Add(key, items);
    }

    // 删除宝箱
    public static void DelectBox(string key)
    {
        boxs.Remove(key);
    }

    // 获取宝箱
    public static Dictionary<byte, Item> GetBox(string key)
    {
        return boxs.ContainsKey(key) ? boxs[key] : null;
    }

    // 修改宝箱
    public static void SetBox(string key, Dictionary<byte, Item> items)
    {
        boxs[key] = items;
    }

    // 判断宝箱是否为空
    public static bool GetBoxEmpty(string key)
    {
        return GetBox(key).Values.All(v => ReferenceEquals(v, null));
    }
}
