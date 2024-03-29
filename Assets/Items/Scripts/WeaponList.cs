﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储所有Weapon对象的信息
/// </summary>
public class WeaponList : MonoBehaviour
{
    // 图标队列
    public List<Sprite> iconList = new();

    public static Dictionary<byte, Weapon> weapons = new();

    private void Awake()
    {
        if(weapons.Count != 0)
        {
            return;
        }
        // 铁剑
        Weapon ironSword = new(1, "IronSword", WeaponType.Sword, 2, 10, 0);
        ironSword.icon = iconList[0];
        ironSword.ID = Item.Count++;
        ironSword.materials.Add(new(BlockList.GetBlock(9).ID, -1, 1));
        ironSword.materials.Add(new(BlockList.GetBlock(3).ID, -1, 2));
        weapons.Add(ironSword.id, ironSword);

        // 法杖
        Weapon scepter = new(2, "Scepter", WeaponType.Scepter, 10, 10, 5);
        scepter.icon = iconList[1];
        scepter.ID = Item.Count++;
        weapons.Add(scepter.id, scepter);

        foreach(var pair in weapons)
        {
            pair.Value.type = Type.Weapon;
            pair.Value.count = -1;
        }
    }

    // 判断是否存在该武器
    public static Weapon GetWeapon(byte id)
    {
        return weapons.ContainsKey(id) ? weapons[id] : null;
    }

    private void OnDestroy()
    {
        //weapons.Clear();
    }
}
