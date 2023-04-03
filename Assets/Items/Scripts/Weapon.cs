using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器类型
/// </summary>
public enum WeaponType
{
    Sword,
    Scepter,
    Bow,
    Pick,
    Axe
}

/// <summary>
/// 武器对象，存储武器所有信息
/// </summary>
public class Weapon : Item
{
     // 物体ID
    public byte id;
    // 武器名字
    public string name;
    // 武器类型
    public WeaponType weaponType;
    // 武器攻击范围
    public float range;
    // 武器伤害
    public int ATK;
    // 武器蓝耗
    public int mana;

    // 武器贴图
    public Material material;
    // 武器图标uv坐标
    public byte u, v;

    public Weapon(byte id, string name, WeaponType weaponType, byte u, byte v, float range, int ATK, int mana)
    {
        this.id = id;
        this.name = name;
        this.weaponType = weaponType;
        this.u = u;
        this.v = v;
        this.range = range;
        this.ATK = ATK;
        this.mana = mana;
    }
}
