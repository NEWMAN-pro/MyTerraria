using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Box : MonoBehaviour
{
    // 物品队列
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();

    private void OnEnable()
    {
        // 显示宝箱界面时将垃圾箱下调
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -765);
    }

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
            if (block == null)
            {
                Debug.Log("该物品为空ba");
                return;
            }
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 50, new Vector3(0, -0.9f, -0.01f));
        }
    }

    // 更改选择框
    public Item Select(int key, ref Item selectItem, ref RectTransform select)
    {
        key -= 60;
        Item item = GetItem((byte)key);
        if (selectItem != null)
        {
            selectItem.flag = false;
            SetItem((byte)key, selectItem);
            if (item == null)
            {
                // 如果该格为空，则置空选择框
                selectItem = null;
                select.gameObject.SetActive(false);
            }
            else
            {
                // 否则更新选择框的物品
                selectItem = item;
                return item;
            }
        }
        else
        {
            // 选择框没有物品，则获取该格物品
            if (item == null)
            {
                return null;
            }
            select.gameObject.SetActive(true);
            selectItem = item;
            SetItem((byte)key, null);
            return item;
        }
        return null;
    }

    // 一键整理
    public void Neaten()
    {
        // 对字典排序，先比较type再比较ID
        Dictionary<byte, Item> sortedItems = items.Where(i => i.Value != null)
            .OrderBy(i => i.Value.type).ThenBy(i => i.Value.ID)
            .ToDictionary(i => i.Key, i => i.Value);
        byte key = 10;
        foreach (var pair in sortedItems)
        {
            // 先清空
            SetItem(pair.Key, null);
        }
        foreach (var pair in sortedItems)
        {
            while (GetItem(key) != null)
            {
                // 找到不为空的背包格
                key++;
            }
            // 再重新绘制
            SetItem(key++, pair.Value);
        }
    }

    private void OnDisable()
    {
        // 隐藏宝箱界面时将垃圾箱归位
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -262.57f);
    }
}
