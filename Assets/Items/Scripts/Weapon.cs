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
    // 武器类型
    public WeaponType weaponType;
    // 武器攻击范围
    public float range;
    // 武器伤害
    public int ATK;
    // 武器蓝耗
    public int mana;

    // 武器贴图
    public Sprite icon;

    public Weapon(byte id, string name, WeaponType weaponType, float range, int ATK, int mana)
    {
        this.id = id;
        this.name = name;
        this.weaponType = weaponType;
        this.range = range;
        this.ATK = ATK;
        this.mana = mana;
    }
}
