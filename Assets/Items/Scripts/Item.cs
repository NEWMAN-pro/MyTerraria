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
    Other = 7
}

public class Item : IComparable<Item>
{
    // ��ƷID
    public byte ID;
    // ��Ʒ����
    public Type type;
    // ��Ʒ����
    public int count;
    // �Ƿ񱻱��
    public bool flag;

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
