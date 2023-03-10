using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Box : MonoBehaviour
{
    // 宝箱密钥
    public string key;
    // 物品队列
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();

    private void OnEnable()
    {
        // 显示宝箱界面时将垃圾箱下调
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -765);
        foreach(var pair in items)
        {
            // 重新绘制宝箱物品
            CreateUI(pair.Value, pair.Key);
        }
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
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
        }
    }

    // 更改选择框
    public Item Select(int key, ref Item selectItem, ref RectTransform select, ref bool flag)
    {
        key -= 60;
        Item item = GetItem((byte)key);
        if (Input.GetKey(KeyCode.LeftShift) && item != null)
        {
            // 如果是按下LeftShift键，则将该格物品放入背包，并将该格置空
            SetItem((byte)key, null);
            flag = true;
            return item;
        }
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

    // 存入物品
    public bool Storage(Item item)
    {
        // 找到第一个为空的空格
        byte key = items.FirstOrDefault(x => x.Value == null).Key;
        Item item1 = GetItem(key);
        if(item1 != null)
        {
            // 如果当前队列没找到空格
            // 背包放满了
            return false;
        }
        SetItem(key, item);
        return true;
    }

    private void OnDisable()
    {
        // 隐藏宝箱界面时将垃圾箱归位
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -262.57f);
        // 隐藏时清空队列
        BoxList.SetBox(key, items);
        foreach(var pair in items)
        {
            CreateUI(null, pair.Key);
        }
    }
}
