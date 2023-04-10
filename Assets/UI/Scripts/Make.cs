using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Make : MonoBehaviour
{
    // 合成物队列
    public Dictionary<byte, Item> synthesis = new();
    // 材料队列
    public Dictionary<byte, Item> materials = new();
    // 所选材料
    [NonSerialized]
    public Item material;
    // 滚动视图框
    public Transform content;
    // 物品框预制体
    public GameObject itemPrefab;
    //public GameObject item;

    private void Awake()
    {
        itemPrefab = Resources.Load("Prefabs/Item") as GameObject;
        content = this.transform.GetChild(2).GetChild(0).GetChild(0);
        //// 初始生成48个Item框
        //for (byte i = 0; i < 48; i++)
        //{
        //    GameObject item = Instantiate(itemPrefab, content);
        //    item.name = "Item " + i.ToString();
        //    item.SetActive(false);
        //}
        byte cnt = 0;
        foreach (var pair in BlockList.blocks)
        {
            Item item = pair.Value;
            item.type = Type.Block;
            item.ID = item.id;
            item.count = -1;
            synthesis[cnt] = item;
            GameObject itemBox = Instantiate(itemPrefab, this.transform.GetChild(2).GetChild(0).GetChild(0));
            itemBox.name = "item " + cnt.ToString();
            itemBox.SetActive(false);
            CreateUI(item, cnt, this.transform.GetChild(2).GetChild(0).GetChild(0));
            cnt++;
        }
        foreach (var pair in WeaponList.weapons)
        {
            Item item = pair.Value;
            item.type = Type.Weapon;
            item.ID = item.id;
            item.count = -1;
            synthesis[cnt] = item;
            GameObject itemBox = Instantiate(itemPrefab, this.transform.GetChild(2).GetChild(0).GetChild(0));
            itemBox.name = "item " + cnt.ToString();
            itemBox.SetActive(false);
            CreateUI(item, cnt, this.transform.GetChild(2).GetChild(0).GetChild(0));
            cnt++;
        }
    }

    private void OnEnable()
    {
        CreateSynthesis();
    }

    // 根据所选材料生成合成物
    public void CreateSynthesis()
    {
        if(material == null)
        {
            // 没有所选材料，则将所有物品加载
            int cnt = BlockList.blocks.Count + WeaponList.weapons.Count;
            for(int i = 0; i < cnt; i++)
            {
                content.GetChild(i).gameObject.SetActive(true);
            }
            content.GetComponent<RectTransform>().sizeDelta = new(0, (cnt / 8 + 1) * 100);
        }
        else
        {

        }
    }

    // 绘制图标
    public void CreateUI(Item item, byte key, Transform trans)
    {
        trans.GetChild(key).GetComponent<CreateUI>().HideUI();
        trans.GetChild(key).GetChild(0).GetComponent<Text>().text = "";
        if(item == null)
        {
            trans.GetChild(key).GetComponent<CreateUI>().CreateBlank();
            return;
        }
        if(item.type == Type.Block)
        {
            // 如果是绘制方块图标
            Block block = BlockList.GetBlock(item.ID);
            if(block == null)
            {
                return;
            }
            trans.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
        }
        else if(item.type == Type.Weapon)
        {
            // 如果是绘制武器图标
            Weapon weapon = WeaponList.GetWeapon(item.ID);
            if(weapon != null)
            {
                trans.GetChild(key).GetComponent<CreateUI>().CreateWeaponUI(weapon.icon);
            }
        }
    }
}
