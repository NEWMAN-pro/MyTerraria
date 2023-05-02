using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class Backpack : MonoBehaviour
{
    // 物品队列
    public Dictionary<byte, Item> items = new();
    // 物品栏
    public Inventory inventory;
    // 宝箱
    public Box box;
    // 选择框
    public RectTransform select;
    // 当前选择的物品
    [NonSerialized]
    public Item selectItem;
    // 未标记的颜色
    public Color NotFlag;
    // 标记的颜色
    public Color FlagColor;
    // 物品数量text
    public GameObject textPrefab;
    public GameObject text;
    // 物品图标
    public GameObject iconPrefab;
    public GameObject icon;

    private void Awake()
    {
        // 物品数量初始化
        textPrefab = Resources.Load("Prefabs/ItemCount") as GameObject;
        // 物品图标初始化
        iconPrefab = Resources.Load("Prefabs/Icon") as GameObject;
        for (byte i = 0; i < 60; i++)
        {
            items[i] = null;
            text = Instantiate(textPrefab, this.transform.GetChild(i));
            icon = Instantiate(iconPrefab, this.transform.GetChild(i));
            icon.name = "Icon";
            text.name = "ItemCount";
            if (i > 50 && i < 59)
            {
                // 如果是金币格或者弹药格，则调整文本框位置
                text.GetComponent<RectTransform>().anchoredPosition = new Vector3(20f, -15f, -0.4f);
                // 调整图标大小
                icon.GetComponent<RectTransform>().sizeDelta = new(60, 60);
            }
            if(i < 10)
            {
                text = Instantiate(textPrefab, inventory.transform.GetChild(i));
                icon = Instantiate(iconPrefab, inventory.transform.GetChild(i));
                icon.name = "Icon";
                text.name = "ItemCount";
            }
        }
        if (!StartUI.flag)
        {
            // 如果是继续游戏
            items = AccessGameAll.data.items;
            foreach(var pair in items)
            {
                CreateUI(pair.Value, pair.Key, false);
            }
            return;
        }

        items[0] = new(BlockList.GetBlock(3));
        items[0].count = 60;
        inventory.SetItem(1, items[0]);
        CreateUI(items[0], 0, false);
        items[1] = new(BlockList.GetBlock(1));
        items[1].count = 60;
        inventory.SetItem(2, items[1]);
        CreateUI(items[1], 1, false);
        items[2] = new(BlockList.GetBlock(2));
        items[2].count = 60;
        inventory.SetItem(3, items[2]);
        CreateUI(items[2], 2, false);
        items[3] = new(BlockList.GetBlock(4));
        items[3].count = 60;
        inventory.SetItem(4, items[3]);
        CreateUI(items[3], 3, false);
        items[4] = new(BlockList.GetBlock(5));
        items[4].count = 60;
        inventory.SetItem(5, items[4]);
        CreateUI(items[4], 4, false);
        items[5] = new(BlockList.GetBlock(6));
        items[5].count = 60;
        inventory.SetItem(6, items[5]);
        CreateUI(items[5], 5, false);
        items[6] = new(BlockList.GetBlock(7));
        items[6].count = 60;
        inventory.SetItem(7, items[6]);
        CreateUI(items[6], 6, false);
        items[7] = new(BlockList.GetBlock(8));
        items[7].count = 60;
        inventory.SetItem(8, items[7]);
        CreateUI(items[7], 7, false);
        items[8] = new(BlockList.GetBlock(3));
        items[8].count = 60;
        inventory.SetItem(9, items[8]);
        CreateUI(items[8], 8, false);
        items[9] = new(WeaponList.GetWeapon(1));
        inventory.SetItem(0, items[9]);
        CreateUI(items[9], 9, false);
        items[10] = new(WeaponList.GetWeapon(2));
        CreateUI(items[10], 10, false);
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
        if (!selectFlag)
        {
            this.transform.GetChild(key).GetComponent<CreateUI>().HideUI();
            this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = "";
        }
        if(item == null)
        {
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlank();
            if (selectFlag)
            {
                select.GetChild(0).GetComponent<Text>().text = "";
            }
            return;
        }
        if (item.type == Type.Block)
        {
            Block block = BlockList.GetBlock(item.id);
            if (block == null)
            {
                Debug.Log("该物品为空ba");
                return;
            }
            if (selectFlag)
            {
                select.GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
                if (item.count != -1) select.GetChild(0).GetComponent<Text>().text = item.count.ToString();
                return;
            }
            else
            {
                this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
            }
        }
        else if(item.type == Type.Weapon)
        {
            Weapon weapon = WeaponList.GetWeapon(item.id);
            if(weapon != null)
            {
                if (selectFlag)
                {
                    select.GetComponent<CreateUI>().CreateWeaponUI(weapon.icon);
                    return;
                }
                else
                {
                    this.transform.GetChild(key).GetComponent<CreateUI>().CreateWeaponUI(weapon.icon);
                }
            }
        }
        SetColor(key, item.flag);
        if (item.count != -1) this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = item.count.ToString();
    }

    // 更改选择框
    public void Select(int key)
    {
        if(key >= 60)
        {
            int flag = 0;
            Item item_ = box.Select(key, ref selectItem, ref select, ref flag);
            if(item_ != null)
            {
                if (flag == 1)
                {
                    // 如果是存入物品
                    Storage(item_);
                }
                else if(flag == 2)
                {
                    // 如果是删除物品
                    SetItem(50, item_);
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
        if(Input.GetKey(KeyCode.LeftControl) && item != null && !item.flag && key != 50)
        {
            // 如果是按下LeftControl键，并且该物品未被标记，且该格不是垃圾桶，则将物品移至垃圾桶
            SetItem(50, item);
            // 将本格清空
            SetItem((byte)key, null);
            return;
        }
        if(Input.GetKey(KeyCode.LeftShift) && item != null && !item.flag && !GameObject.Find("UI").GetComponent<UI>().boxFlag)
        {
            // 如果是按下LeftShift键并且打开了宝箱界面，并且该物品未被标记，则将该格物品存入背包
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
            //SetColor(key, selectItem.flag);
            // 先清空选择框中的物品
            CreateUI(null, 59, false);
            if (item == null || key == 50)
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
        // 对字典排序，先比较type再比较ID（从小到大），再比较flag（true排前），再比较key（从小到大）
        var sortedItems_ = items.Where(i => i.Value != null)
           .OrderBy(i => i.Value.type).ThenBy(i => i.Value.ID).ThenByDescending(i => i.Value.flag).ThenBy(i => i.Key)
           .ToList();
        for(int i = 0; i < sortedItems_.Count; i++)
        {
            // 进行合并
            var pair = sortedItems_[i];
            if (i != sortedItems_.Count - 1)
            {
                var nextPair = sortedItems_[i + 1];
                if (pair.Value.type == nextPair.Value.type && pair.Value.ID == nextPair.Value.ID)
                {
                    // 两个物品的总数量
                    int ans = pair.Value.count + nextPair.Value.count;
                    if (ans <= 64)
                    {
                        pair.Value.count = ans;
                        // 数量为0，删除物品
                        sortedItems_[i + 1].Value.count = 0;
                        SetItem(nextPair.Key, null);
                    }
                    else
                    {
                        pair.Value.count = 64;
                        sortedItems_[i + 1].Value.count = ans - 64;
                    }
                }
            }
            // 将物品栏与被标记的物品重新绘制
            if(pair.Key < 10 || pair.Value.flag)
            {
                if(pair.Value.count == 0)
                {
                    // 数量为0，不绘制
                    continue;
                }
                SetItem(pair.Key, pair.Value);
            }
        }
        // 将物品栏与被标记的物品以及数量为0的物品剔除
        var sortedItems = sortedItems_.Where(i => i.Value != null && i.Value.count != 0 && i.Key > 9 && !i.Value.flag)
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
        // 找出所有同类物品
        var result = items.Where(x => x.Value != null && x.Value.ID == item.ID).ToList();
        foreach(var pair in result)
        {
            Debug.Log(pair.Key + " " + pair.Value.count);
            Item item_ = pair.Value;
            int ans = item_.count + item.count;
            item_.count = Math.Min(64, ans);
            SetItem(pair.Key, item_);
            ans -= 64;
            if(ans > 0)
            {
                item.count = ans;
            }
            else
            {
                // 将所有物品存入，则退出
                return true;
            }
        }
        byte key;
        // 找到第一个为空的空格
        key = items.FirstOrDefault(x => x.Value == null).Key;
        if (GetItem(key) != null)
        {
            // 如果当前队列没找到空格
            // 背包放满了
            return false;
        }
        SetItem(key, item);
        return true;
    }

    // 存放全部
    public void Deposit()
    {
        var items_ = items.Where(i => i.Value != null && i.Key > 9 && i.Key != 50 && !i.Value.flag).ToDictionary(i => i.Key, i => i.Value);
        foreach(var pair in items_)
        {
            box.Storage(pair.Value);
        }
        foreach(var pair in items_)
        {
            SetItem(pair.Key, null);
        }
    }

    // 快速堆叠
    public void Stack()
    {
        // 找到背包与宝箱中相同的物品，且未被标记
        // 垃圾桶中物品不参与匹配
        var items_ = items.Where(i => i.Key != 50 && i.Value != null && !i.Value.flag).ToDictionary(i => i.Key, i => i.Value);
        var commonItems = items_.Values.Intersect(box.items.Values, new ItemEqualityComparer()).ToList();
        var commonItemsWithKeys = items_.Where(pair => commonItems.Contains(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
        foreach (var pair in commonItemsWithKeys)
        {
            box.Storage(pair.Value);
            SetItem(pair.Key, null);
        }
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

// 自定义比较器
public class ItemEqualityComparer : IEqualityComparer<Item>
{
    public bool Equals(Item x, Item y)
    {
        if (x == null || y == null)
        {
            return false;
        }
        else
        {
            return x.ID == y.ID && x.type == y.type;
        }
    }

    public int GetHashCode(Item obj)
    {
        return obj.ID.GetHashCode() ^ obj.type.GetHashCode();
    }
}
