using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // 物品栏是否有物品改变
    public byte selectID = 1;

    // 物品队列
    public Dictionary<byte, Item> items = new()
    {
        { 1, null },
        { 2, null },
        { 3, null },
        { 4, null },
        { 5, null },
        { 6, null },
        { 7, null },
        { 8, null },
        { 9, null },
        { 0, null },
    };
    //public Item[] items = new Item[10];
    // 选择框
    public RectTransform select;

    // Update is called once per frame
    void Update()
    {

    }

    // 获取物品
    public Item GetItem(byte key)
    {
        return items.ContainsKey(key) ? items[key] : null;
    }

    // 修改物品
    public void SetItem(byte key, Item item)
    {
        items[key] = item;
        CreateUI(item, key);
        selectID = key;
    }

    // 绘制
    public void CreateUI(Item item, byte key)
    {
        if (item == null)
        {
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlank();
            return;
        }
        if (item.type == Type.Block)
        {
            Block block = BlockList.GetBlock(item.ID);
            if(block == null)
            {
                Debug.Log("该物品为空in");
                return;
            }
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
        }
    }

    // 移动选择框
    public void SetSelect(byte key)
    {
        if(key == 0)
        {
            key = 10;
        }
        select.anchoredPosition = new Vector2((key - 5) * 100 - 50, 0);
    }
}
