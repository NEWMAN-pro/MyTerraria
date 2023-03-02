using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // 物品队列
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();
    // 选择框
    public RectTransform select;

    private void Awake()
    {
        Item item_1 = new Item();
        item_1.type = 0;
        item_1.ID = 3;
        items.Add(1, item_1);
        CreateUI(item_1, 1);
        Item item_2 = new Item();
        item_2.type = 0;
        item_2.ID = 1;
        items.Add(2, item_2);
        CreateUI(item_2, 2);
        Item item_3 = new Item();
        item_3.type = 0;
        item_3.ID = 2;
        items.Add(3, item_3);
        CreateUI(item_3, 3);
        Item item_4 = new Item();
        item_4.type = 0;
        item_4.ID = 4;
        items.Add(4, item_4);
        CreateUI(item_4, 4);
        Item item_5 = new Item();
        item_5.type = 0;
        item_5.ID = 5;
        items.Add(5, item_5);
        CreateUI(item_5, 5);
        Item item_6 = new Item();
        item_6.type = 0;
        item_6.ID = 6;
        items.Add(6, item_6);
        CreateUI(item_6, 6);
        Item item_7 = new Item();
        item_7.type = 0;
        item_7.ID = 7;
        items.Add(7, item_7);
        CreateUI(item_7, 7);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Item GetItem(byte key)
    {
        return items.ContainsKey(key) ? items[key] : null;
    }

    public void SetItem(byte key, Item item)
    {
        items[key] = item;
    }

    public void CreateUI(Item item, byte key)
    {
        if(item.type == Type.Block)
        {
            Block block = BlockList.GetBlock(item.ID);
            this.transform.GetChild(key).GetComponent<CreateBlockUI>().CreateUI(block, true, 50, new Vector3(0, -0.9f, -0.01f));
        }
    }

    public void SetSelect(byte key)
    {
        select.anchoredPosition = new Vector2((key - 5) * 100 - 50, 0);
    }
}
