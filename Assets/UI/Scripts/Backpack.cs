using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    // 物品队列
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();
    // 物品栏
    public Inventory inventory;
    // 选择框
    public CreateUI selectCreateUI;

    private void Awake()
    {
        Item item_1 = new Item();
        item_1.type = 0;
        item_1.ID = 3;
        items.Add(0, item_1);
        inventory.SetItem(1, item_1);
        CreateUI(item_1, 0, false);
        Item item_2 = new Item();
        item_2.type = 0;
        item_2.ID = 1;
        items.Add(1, item_2);
        inventory.SetItem(2, item_2);
        CreateUI(item_2, 1, false);
        Item item_3 = new Item();
        item_3.type = 0;
        item_3.ID = 2;
        items.Add(2, item_3);
        inventory.SetItem(3, item_3);
        CreateUI(item_3, 2, false);
        Item item_4 = new Item();
        item_4.type = 0;
        item_4.ID = 4;
        items.Add(3, item_4);
        inventory.SetItem(4, item_4);
        CreateUI(item_4, 3, false);
        Item item_5 = new Item();
        item_5.type = 0;
        item_5.ID = 5;
        items.Add(4, item_5);
        inventory.SetItem(5, item_5);
        CreateUI(item_5, 4, false);
        Item item_6 = new Item();
        item_6.type = 0;
        item_6.ID = 6;
        items.Add(5, item_6);
        inventory.SetItem(6, item_6);
        CreateUI(item_6, 5, false);
        Item item_7 = new Item();
        item_7.type = 0;
        item_7.ID = 7;
        items.Add(6, item_7);
        inventory.SetItem(7, item_7);
        CreateUI(item_7, 6, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        CreateUI(item, key, false);
        if(key <= 9)
        {
            // 同步更新物品栏
            byte newKey = (byte)(key + 1);
            if(newKey == 10)
            {
                newKey = 0;
            }
            inventory.SetItem(newKey, item);
        }
    }

    // 绘制
    public void CreateUI(Item item, byte key, bool select)
    {
        if (item.type == Type.Block)
        {
            Block block = BlockList.GetBlock(item.ID);
            if (block == null)
            {
                Debug.Log("该物品为空ba");
                return;
            }
            if (select)
            {
                selectCreateUI.CreateBlockUI(block, true, 50, new Vector3(0, -0.9f, -0.01f));
            }
            else
            {
                this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 50, new Vector3(0, -0.9f, -0.01f));
            }
        }
    }

    public void Select(int key)
    {
        Item item = GetItem((byte)key);
        if (item == null) return;
        CreateUI(item, (byte)key, true);
    }
}
