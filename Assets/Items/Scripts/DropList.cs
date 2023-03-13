using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;

public class DropList : MonoBehaviour
{
    // 掉落物上限
    public static int maxDrop = 1000;
    // 掉落物对象队列
    public static GameObject[] dropGBs = new GameObject[maxDrop];
    // 掉落物队列
    public static PriorityQueue<Drop> drops;
    // 掉落物存在时限 s
    public float maxTime = 60;

    private void Start()
    {
        GameObject gameObject = Resources.Load("Prefabs/Drop") as GameObject;
        // 初始化对象队列
        for (int i = 0; i < maxDrop; i++)
        {
            GameObject newGB = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
            dropGBs[i] = newGB;
        }
    }

    private void Update()
    {
        float time = Time.deltaTime;
        foreach(var drop in drops.heap)
        {
            drop.time += time;
        }

        // 将超时的掉落物删除
        while(drops.Peek().time >= maxTime)
        {
            drops.Dequeue();
        }
    }

    // 增加掉落物
    public static void AddDrop(Item item)
    {
        if(drops.Count >= maxDrop)
        {
            // 到达上限，删除堆顶
            drops.Dequeue();
        }
        int id = GetGB();
        CreateDrop(item, id, false);
        Drop drop = new(id, item);
        drops.Enqueue(drop);
    }

    // 删除掉落物
    public static void Destroy(Drop drop)
    {
        if (!drops.Remove(drop))
        {
            Debug.Log("不存在该物品");
            return;
        }
        dropGBs[drop.id].SetActive(false);
        CreateDrop(drop.item, drop.id, true);
    }

    // 找到对象队列第一个为空的值
    public static int GetGB()
    {
        for(int i = 0; i < maxDrop; i++)
        {
            if (!dropGBs[i].activeSelf)
            {
                dropGBs[i].SetActive(true);
                // 找到返回
                return i;
            }
        }
        // 没有返回-1
        return -1;
    }

    // 绘制掉落物
    public static void CreateDrop(Item item, int id, bool flag)
    {
        if (flag)
        {
            // 如果是删除，将贴图置空
            dropGBs[id].GetComponent<CreateUI>().CreateBlank();
            return;
        }
        if(item.type == Type.Block)
        {
            // 如果是方块
            Block block = BlockList.GetBlock(item.ID);
            dropGBs[id].GetComponent<CreateUI>().CreateBlockDrop(block, 1);
        }
    }
}
