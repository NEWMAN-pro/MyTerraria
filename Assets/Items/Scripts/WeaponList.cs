using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储所有Weapon对象的信息
/// </summary>
public class WeaponList : MonoBehaviour
{
    public static Dictionary<byte, Weapon> weapons = new();

    private void Awake()
    {
        // 铁剑
        Weapon ironSword = new(1, "IronSword", WeaponType.Sword, 13, 30, 2, 10, 0);
        weapons.Add(ironSword.id, ironSword);
    }

    // 判断是否存在该武器
    public static Weapon GetWeapon(byte id)
    {
        return weapons.ContainsKey(id) ? weapons[id] : null;
    }

    private void OnDestroy()
    {
        weapons.Clear();
    }
}
