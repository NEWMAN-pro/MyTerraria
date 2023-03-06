using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type : byte
{
    Block = 0,
    Weapon = 1,
    Furniture = 2,
    Buff = 3,
    Money = 4,
    Ammo = 5,
    Other = 7
}

public class Item
{
    // 物品ID
    public byte ID;
    // 物品类型
    public Type type;
    // 物品数量
    public int count;
    // 是否被标记
    public bool flag;
}
