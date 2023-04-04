using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // 物品栏是否有物品改变
    public byte selectID = 1;
    // 物品名字文本
    public Text itemName;
    // 选择框颜色
    public Color FlagColor;
    // 正常颜色
    public Color NotFlag;

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
    // 物品数量text
    public GameObject textPrefab;
    public GameObject text;

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
        this.transform.GetChild(key).GetComponent<CreateUI>().HideUI();
        this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = "";
        if (item == null)
        {
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlank();
            this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = "";
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
        else if (item.type == Type.Weapon)
        {
            Weapon weapon = WeaponList.GetWeapon(item.ID);
            if (weapon != null)
            {
                this.transform.GetChild(key).GetComponent<CreateUI>().CreateWeaponUI(weapon.icon);
            }
        }
        if (item.count != -1) this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = item.count.ToString();
    }

    // 移动选择框
    public void SetSelect(byte key)
    {
        for(byte i = 0; i < 10; i++)
        {
            if(i == key)
            {
                continue;
            }
            this.transform.GetChild(i).GetComponent<Image>().color = NotFlag;
        }
        this.transform.GetChild(key).GetComponent<Image>().color = FlagColor;
        Item item = GetItem(key);
        if(item != null)
        {
            itemName.text = item.GetName();
        }
        else
        {
            itemName.text = "";
        }
    }
}
