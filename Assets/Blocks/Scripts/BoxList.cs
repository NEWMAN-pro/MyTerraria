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
            // 如果是继续游戏
            boxs = AccessGameAll.data.boxs;
            boxsName = AccessGameAll.data.boxsName;
            Debug.Log(boxs.Count);
        }
    }

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
        boxsName.Add(key, "宝箱");
    }

    // 删除宝箱
    public static void DelectBox(string key)
    {
        boxs.Remove(key);
        boxsName.Remove(key);
    }

    // 获取宝箱
    public static Dictionary<byte, Item> GetBox(string key, out string name)
    {
        name = boxsName.ContainsKey(key) ? boxsName[key] : "";
        return boxs.ContainsKey(key) ? boxs[key] : null;
    }

    // 修改宝箱
    public static void SetBox(string key, Dictionary<byte, Item> items, string name)
    {
        boxs[key] = items;
        boxsName[key] = name;
    }

    // 判断宝箱是否为空
    public static bool GetBoxEmpty(string key)
    {
        string name;
        return GetBox(key, out name).Values.All(v => ReferenceEquals(v, null));
    }

    // 销毁脚本时
    private void OnDestroy()
    {
        if(boxs.Count != 0)
        {
            boxs.Clear();
            boxsName.Clear();
        }
    }
}
