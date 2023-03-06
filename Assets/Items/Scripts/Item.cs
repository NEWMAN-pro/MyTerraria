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
    // ��ƷID
    public byte ID;
    // ��Ʒ����
    public Type type;
    // ��Ʒ����
    public int count;
    // �Ƿ񱻱��
    public bool flag;
}
