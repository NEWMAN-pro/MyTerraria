using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Drop : IComparable<Drop>
{
    public int id;
    public Item item;
    public float time;

    public Drop(int id, Item item)
    {
        this.id = id;
        this.item = item;
        time = 0f;
    }

    public int CompareTo(Drop other)
    {
        // 按时间从大到小排序
        if (this.time > other.time)
        {
            return -1;
        }
        else if (this.time < other.time)
        {
            return 1;
        }
        else return 0;
    }
}