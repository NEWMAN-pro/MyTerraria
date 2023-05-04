using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Make : MonoBehaviour
{
    // 合成物队列
    public Dictionary<byte, Item> synthesis = new();
    // 材料队列
    public Dictionary<byte, Item> materials = new();
    // 显示的合成物队列
    public List<int> showItems = new();
    // 所选材料
    public int material = -1;
    // 滚动视图框
    public Transform content;
    // 背包
    public Transform backpack;
    // 物品框预制体
    public GameObject itemPrefab;
    // 材料框数量
    public int materialCount;

    private void Awake()
    {
        itemPrefab = Resources.Load("Prefabs/Item") as GameObject;
        content = this.transform.GetChild(2).GetChild(0).GetChild(0);
        byte cnt = 0;
        foreach (var pair in BlockList.blocks)
        {
            Item item = new(pair.Value);
            item.count = -1;
            synthesis[cnt] = item;
            GameObject itemBox = Instantiate(itemPrefab, this.transform.GetChild(2).GetChild(0).GetChild(0));
            itemBox.name = "item " + cnt.ToString();
            itemBox.SetActive(false);
            CreateUI(item, cnt, this.transform.GetChild(2).GetChild(0).GetChild(0));
            cnt++;
            showItems.Add(item.ID);
        }
        foreach (var pair in WeaponList.weapons)
        {
            Item item = new(pair.Value);
            item.count = -1;
            synthesis[cnt] = item;
            GameObject itemBox = Instantiate(itemPrefab, this.transform.GetChild(2).GetChild(0).GetChild(0));
            itemBox.name = "item " + cnt.ToString();
            itemBox.SetActive(false);
            CreateUI(item, cnt, this.transform.GetChild(2).GetChild(0).GetChild(0));
            cnt++;
            showItems.Add(item.ID);
        }
    }

    private void OnEnable()
    {
        material = -1;
        CreateSynthesis();
    }

    // 按物品类型查找
    public void TypeQuery(string type)
    {
        // 先找出所有合成物
        CreateSynthesis();
        Show(false);

        Type type_ = Type.Other;
        if (type == "Weapons")
        {
            type_ = Type.Weapon;
        }
        else if (type == "Blocks")
        {
            type_ = Type.Block;
        }
        else if (type == "ALL")
        {
            Show(true);
            return;
        }
        // 找出所有已显示的物品
        var result = synthesis.Where(pair => showItems.Contains(pair.Key)).ToList();
        // 找出所有与所选择的类型相同的物品
        var result2 = result.Where(pair => pair.Value.type == type_).ToList();
        showItems.Clear();
        foreach (var pair in result2)
        {
            showItems.Add(pair.Key);
        }
        Show(true);
    }

    // 根据所选材料生成合成物
    public void CreateSynthesis()
    {
        int cnt = Item.Count;

        // 先将Show列表中的物品取消显示
        Show(false);
        showItems.Clear();

        if (material == -1)
        {
            // 没有所选材料，则将所有物品加载
            for (int i = 0; i < cnt; i++)
            {
                showItems.Add(i);
            }
        }
        else
        {
            // 找到所有合成材料包含material的物品
            var query = from item in BlockList.blocks.Values.Cast<Item>().Concat(WeaponList.weapons.Values)
                        where item.materials.Any(m => m.id == material)
                        select item;
            foreach (var pair in query)
            {
                // 找出对应ID的物品
                var item = synthesis.FirstOrDefault(item_ => item_.Value.ID == pair.ID);
                if (item.Value != null)
                {
                    // 显示出来
                    showItems.Add(item.Key);
                }
            }
            // 将自己也显示出来
            showItems.Add(material);
        }
        Show(true);
    }

    // 将显示列表中的物品显示
    public void Show(bool flag)
    {
        foreach (var id in showItems)
        {
            content.GetChild(id).gameObject.SetActive(flag);
        }
        content.GetComponent<RectTransform>().sizeDelta = new(0, (showItems.Count / 8 + 1) * 100);
    }

    // 更改所选材料
    public void SetMaterial()
    {
        if (backpack.GetChild(59).gameObject.activeSelf)
        {
            // 如果选择框处于激活状态，则获取选择框物品
            Item selectItem = backpack.GetComponent<Backpack>().selectItem;
            material = selectItem.ID;
            CreateSynthesis();
        }
    }

    // 绘制图标
    public void CreateUI(Item item, byte key, Transform trans)
    {
        trans.GetChild(key).GetComponent<CreateUI>().HideUI();
        trans.GetChild(key).GetChild(0).GetComponent<Text>().text = "";
        if (item == null)
        {
            trans.GetChild(key).GetComponent<CreateUI>().CreateBlank();
            return;
        }
        if (item.type == Type.Block)
        {
            // 如果是绘制方块图标
            Block block = BlockList.GetBlock(item.id);
            if (block == null)
            {
                return;
            }
            trans.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
        }
        else if (item.type == Type.Weapon)
        {
            // 如果是绘制武器图标
            Weapon weapon = WeaponList.GetWeapon(item.id);
            if (weapon != null)
            {
                trans.GetChild(key).GetComponent<CreateUI>().CreateWeaponUI(weapon.icon);
            }
        }
    }
}
