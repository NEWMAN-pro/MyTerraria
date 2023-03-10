using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Backpack : MonoBehaviour
{
    // 物品队列
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();
    // 物品栏
    public Inventory inventory;
    // 宝箱
    public Box box;
    // 选择框
    public RectTransform select;
    // 当前选择的物品
    public Item selectItem;
    // 未标记的颜色
    public Color NotFlag;
    // 标记的颜色
    public Color FlagColor;

    private void Awake()
    {
        for(byte i = 0; i < 60; i++)
        {
            items[i] = null;
        }

        Item item_1 = new Item();
        item_1.type = 0;
        item_1.ID = 3;
        //items.Add(0, item_1);
        items[0] = item_1;
        inventory.SetItem(1, item_1);
        CreateUI(item_1, 0, false);
        Item item_2 = new Item();
        item_2.type = 0;
        item_2.ID = 1;
        items[1] = item_2;
        inventory.SetItem(2, item_2);
        CreateUI(item_2, 1, false);
        Item item_3 = new Item();
        item_3.type = 0;
        item_3.ID = 2;
        items[2] = item_3;
        inventory.SetItem(3, item_3);
        CreateUI(item_3, 2, false);
        Item item_4 = new Item();
        item_4.type = 0;
        item_4.ID = 4;
        items[3] = item_4;
        inventory.SetItem(4, item_4);
        CreateUI(item_4, 3, false);
        Item item_5 = new Item();
        item_5.type = 0;
        item_5.ID = 5;
        items[4] = item_5;
        inventory.SetItem(5, item_5);
        CreateUI(item_5, 4, false);
        Item item_6 = new Item();
        item_6.type = 0;
        item_6.ID = 6;
        items[5] = item_6;
        inventory.SetItem(6, item_6);
        CreateUI(item_6, 5, false);
        Item item_7 = new Item();
        item_7.type = 0;
        item_7.ID = 7;
        items[6] = item_7;
        inventory.SetItem(7, item_7);
        CreateUI(item_7, 6, false);
        Item item_8 = new Item();
        item_8.type = 0;
        item_8.ID = 8;
        items[7] = item_8;
        inventory.SetItem(8, item_8);
        CreateUI(item_8, 7, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(selectItem != null)
        {
            // 让选择框随鼠标移动
            select.anchoredPosition = (((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2f) * 1920f / (float)Screen.width - this.transform.GetComponent<RectTransform>().anchoredPosition) * 1.25f;
        }

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
    public void CreateUI(Item item, byte key, bool selectFlag)
    {
        if(item == null)
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
            if (selectFlag)
            {
                select.GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
            }
            else
            {
                this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
            }
        }
    }

    // 更改选择框
    public void Select(int key)
    {
        if(key >= 60)
        {
            bool flag = false;
            Item item_ = box.Select(key, ref selectItem, ref select, ref flag);
            if(item_ != null)
            {
                if (flag)
                {
                    // 如果是存入物品
                    Storage(item_);
                }
                else
                {
                    // 选择框获取背包中的物品
                    CreateUI(item_, (byte)key, true);
                }
            }
            return;
        }
        Item item = GetItem((byte)key);
        if (Input.GetKey(KeyCode.LeftAlt) && item != null && key < 50)
        {
            // 如果是按下LeftAlt键，则标记物品
            item.flag = !item.flag;
            SetItem((byte)key, item);
            SetColor(key, item.flag);
            return;
        }
        if(Input.GetKey(KeyCode.LeftControl) && item != null)
        {
            // 如果是按下LeftControl键，则将物品移至垃圾桶
            SetItem(50, item);
            // 将本格清空
            SetItem((byte)key, null);
            return;
        }
        if(Input.GetKey(KeyCode.LeftShift) && item != null && !GameObject.Find("UI").GetComponent<UI>().boxFlag)
        {
            // 如果是按下LeftShift键并且打开了宝箱界面，则将该格物品存入背包
            if (box.Storage(item))
            {
                // 如果放入成功，将本格清空
                SetItem((byte)key, null);
            }
            return;
        }
        if (selectItem != null)
        {
            // 将选择框中的物体物品放到该格
            if(key == 50)
            {
                selectItem.flag = false;
            }
            if(key > 50 && key < 55)
            {
                if(selectItem.type != Type.Money)
                {
                    // 如果物品类型不是金钱则不能放入钱币格
                    return;
                }
            }
            if(key > 55 && key < 59)
            {
                if(selectItem.type != Type.Ammo)
                {
                    // 如果物品类型不是弹药则不能放入弹药格
                    return;
                }
            }
            SetItem((byte)key, selectItem);
            SetColor(key, selectItem.flag);
            if(item == null || key == 50)
            {
                // 如果该格为空，则置空选择框，当选中的是垃圾桶，翻盖其中物品
                selectItem = null;
                select.gameObject.SetActive(false);
            }
            else
            {
                // 否则更新选择框的物品
                CreateUI(item, (byte)key, true);
                selectItem = item;
            }
        }
        else
        {
            // 选择框没有物品，则获取该格物品
            if (item == null)
            {
                return;
            }
            select.gameObject.SetActive(true);
            CreateUI(item, (byte)key, true);
            selectItem = item;
            SetItem((byte)key, null);
            SetColor(key, false);
        }
    }

    // 一键整理
    public void Neaten()
    {
        // 对字典排序，先比较type再比较ID，物品栏不参与排序，被标记的物品不参与排序
        Dictionary<byte, Item> sortedItems = items.Where(i => i.Value != null && i.Key > 9 && !i.Value.flag)
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
            while(GetItem(key) != null)
            {
                // 找到不为空的背包格
                key++;
            }
            // 再重新绘制
            SetItem(key++, pair.Value);
        }
    }

    // 更改物品格颜色
    public void SetColor(int key, bool flag)
    {
        this.transform.GetChild(key).GetComponent<Image>().color = flag ? FlagColor : NotFlag;
    }

    // 存入物品
    public bool Storage(Item item)
    {
        // 找到第一个为空的空格
        byte key = items.FirstOrDefault(x => x.Value == null).Key;
        Item item1 = GetItem(key);
        if (item1 != null)
        {
            // 如果当前队列没找到空格
            // 背包放满了
            return false;
        }
        SetItem(key, item);
        return true;
    }

    // 脚本结束时
    private void OnDisable()
    {
        if(selectItem != null)
        {
            // 如果在选择框未清空的情况下退出，则找到背包中第一个为空的格子，将选择框中的物体放入并清空选择框
            Storage(selectItem);
            selectItem = null;
            select.gameObject.SetActive(false);
        }
    }
}
