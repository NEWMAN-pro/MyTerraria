using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Type : byte
{
    Block = 0,
    Weapon = 1,
    Furniture = 2,
    Buff = 3,
    Money = 4,
    Ammo = 5,
    Other = 7,
    Init = 8
}

/// <summary>
/// 物品总类
/// </summary>
[Serializable]
public class Item : IComparable<Item>
{
    public static byte Count = 0;

    // 物品ID
    public byte ID;
    // 物品类型
    public Type type;
    // 物品数量
    public int count;
    // 是否被标记
    public bool flag;

    // 细化id
    public byte id;
    // 物品名字
    public string name;
    // 合成材料
    public List<ItemMaterial> materials = new();

    public Item(byte ID = 0, Type type = Type.Init, int count = 0, bool flag = false)
    {
        this.ID = ID;
        this.type = type;
        this.count = count;
        this.flag = flag;
    }

    public Item(Block block)
        : this(block.ID, block.type, block.count)
    {
        this.id = block.id;
        this.materials = block.materials;
    }
    public Item(Weapon weapon)
        : this(weapon.ID, weapon.type, weapon.count)
    {
        this.id = weapon.id;
        this.materials = weapon.materials;
    }

    // 获取物品名称
    public string GetName()
    {
        if(type == Type.Block)
        {
            Block block = BlockList.GetBlock(id);
            if(block != null)
            {
                return block.name;
            }
        }
        else if(type == Type.Weapon)
        {
            Weapon weapon = WeaponList.GetWeapon(id);
            if(weapon != null)
            {
                return weapon.name;
            }
        }
        return "";
    }

    // 重载比较方法
    public int CompareTo(Item other)
    {
        if(this.type == other.type)
        {
            if(this.ID < other.ID)
            {
                return -1;
            }
            else if(this.ID > other.ID)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else if(this.type < other.type)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}

/// <summary>
/// 合成物类
/// </summary>
public class ItemMaterial
{
    // 合成物id
    public byte id;
    // 能合成物品个数
    public int x;
    // 所需该合成物个数
    public int y;

    public ItemMaterial(byte id, int x, int y)
    {
        this.id = id;
        this.x = x;
        this.y = y;
    }
}