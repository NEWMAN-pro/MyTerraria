using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type : byte
{
    Block = 0,
    Other = 1
}

public class Item
{
    // 物品ID
    public byte ID;
    // 物品类型
    public Type type;
    // 物品数量
    public int count;

}
