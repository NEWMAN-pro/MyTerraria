using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Box : MonoBehaviour
{
    // 宝箱名字
    public string boxName;
    // 宝箱密钥
    public string key;
    // 物品队列
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();
    // 物品数量text
    public GameObject textPrefab;
    public GameObject text;
    // 物品图标
    public GameObject iconPrefab;
    public GameObject icon;
    // 背包
    public Backpack backpack;

    // 宝箱名字输入框
    public InputField inputField;

    private void OnEnable()
    {
        // 显示宝箱界面时将垃圾箱下调
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -765);
        this.transform.parent.GetChild(61).gameObject.SetActive(false);
        this.transform.parent.GetChild(62).gameObject.SetActive(false);
        foreach(var pair in items)
        {
            // 重新绘制宝箱物品
            if (pair.Value == null) continue;
            CreateUI(pair.Value, pair.Key);
        }
        inputField.text = boxName;
        inputField.interactable = false;
    }

    private void Awake()
    {
        // 物品数量初始化
        textPrefab = Resources.Load("Prefabs/ItemCount") as GameObject;
        // 物品图标初始化
        iconPrefab = Resources.Load("Prefabs/Icon") as GameObject;
        backpack = this.transform.parent.GetComponent<Backpack>();

        bool flag_ = (items.Count != 0);
        for (byte i = 0; i < 50; i++)
        {
            text = Instantiate(textPrefab, this.transform.GetChild(i));
            icon = Instantiate(iconPrefab, this.transform.GetChild(i));
            if (flag_) continue;
            items[i] = null;
            icon.name = "Icon";
            text.name = "ItemCount";
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
        CreateUI(item, key);
    }

    // 绘制
    public void CreateUI(Item item, byte key)
    {
        this.transform.GetChild(key).GetComponent<CreateUI>().HideUI();
        this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = "";
        if (item == null)
        {
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlank();
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
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
        }
        else if(item.type == Type.Weapon)
        {
            Weapon weapon = WeaponList.GetWeapon(item.id);
            if(weapon != null)
            {
                this.transform.GetChild(key).GetComponent<CreateUI>().CreateWeaponUI(weapon.icon);
            }
        }
        if (item.count != -1) this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = item.count.ToString();
    }

    // 更改选择框
    public Item Select(int key, ref Item selectItem, ref RectTransform select, ref int flag)
    {
        key -= 60;
        Item item = GetItem((byte)key);
        if (Input.GetKey(KeyCode.LeftShift) && item != null && !item.flag)
        {
            // 如果是按下LeftShift键，并且该物品未被标记，则将该格物品放入背包，并将该格置空
            SetItem((byte)key, null);
            flag = 1;
            return item;
        }
        if (Input.GetKey(KeyCode.LeftControl) && item != null && !item.flag)
        {
            // 如果是按下LeftControl键，并且该物品未被标记，则将物品移至垃圾桶
            // 将本格清空
            flag = 2;
            SetItem((byte)key, null);
            return item;
        }
        if (selectItem != null)
        {
            selectItem.flag = false;
            SetItem((byte)key, selectItem);
            // 先清空选择框中的物品
            select.transform.GetComponent<CreateUI>().HideUI();
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
        // 对字典排序，先比较type再比较ID（从小到大），再比较key（从小到大）
        var sortedItems_ = items.Where(i => i.Value != null)
           .OrderBy(i => i.Value.type).ThenBy(i => i.Value.ID).ThenBy(i => i.Key)
           .ToList();
        for (int i = 0; i < sortedItems_.Count; i++)
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
        }
        // 对字典排序，先比较type再比较ID
        Dictionary<byte, Item> sortedItems = sortedItems_.Where(i => i.Value != null && i.Value.count != 0)
            .OrderBy(i => i.Value.type).ThenBy(i => i.Value.ID)
            .ToDictionary(i => i.Key, i => i.Value);
        byte key = 0;
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
        var result = items.Where(x => x.Value != null && x.Value.ID == item.ID).ToList();
        foreach (var pair in result)
        {
            Item item_ = pair.Value;
            int ans = item_.count + item.count;
            item_.count = Mathf.Min(64, ans);
            SetItem(pair.Key, item_);
            ans -= 64;
            if (ans > 0)
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
        if(GetItem(key) != null)
        {
            // 如果当前队列没找到空格
            // 箱子放满了
            return false;
        }
        SetItem(key, item);
        return true;
    }

    // 重命名
    public void SetName()
    {
        inputField.interactable = true;
    }

    // 输入结束
    public void EndInput()
    {
        boxName = inputField.text;
        inputField.interactable = false;
    }

    // 强夺全部
    public void Snatch()
    {

        foreach(var pair in items)
        {
            if(pair.Value != null) backpack.Storage(pair.Value);
        }
        for(byte i = 0; i < 50; i++)
        {
            SetItem(i, null);
        }
    }

    // 补货
    public void Replenishment()
    {
        // 找到背包与宝箱中相同的物品，且未被标记
        var items1 = items.Where(i => i.Value != null && !i.Value.flag).ToDictionary(i => i.Key, i => i.Value);
        // 垃圾桶中物品不参与匹配
        var items2 = backpack.items.Where(i => i.Key != 50).ToDictionary(i => i.Key, i => i.Value);
        var commonItems = items1.Values.Intersect(items2.Values, new ItemEqualityComparer()).ToList();
        var commonItemsWithKeys = items1.Where(pair => commonItems.Contains(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
        foreach (var pair in commonItemsWithKeys)
        {
            backpack.Storage(pair.Value);
            SetItem(pair.Key, null);
        }
    }

    private void OnDisable()
    {
        // 隐藏宝箱界面时将垃圾箱归位
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -262.57f);
        this.transform.parent.GetChild(61).gameObject.SetActive(true);
        this.transform.parent.GetChild(62).gameObject.SetActive(true);
        // 隐藏时清空队列
        BoxList.SetBox(key, items, boxName);
        foreach(var pair in items)
        {
            CreateUI(null, pair.Key);
        }
    }
}
