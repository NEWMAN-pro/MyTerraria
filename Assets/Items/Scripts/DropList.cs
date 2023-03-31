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
    // 掉落物类队列
    public static Drop[] dropList = new Drop[maxDrop];
    // 掉落物队列
    public static PriorityQueue<Drop> drops = new();
    // 掉落物存在时限 s
    public float maxTime = 60;

    private void Start()
    {
        GameObject gameObject = Resources.Load("Prefabs/Drop") as GameObject;
        // 初始化对象队列
        for (int i = 0; i < maxDrop; i++)
        {
            GameObject newGB = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
            newGB.name = i.ToString();
            newGB.transform.SetParent(this.transform);
            newGB.SetActive(false);
            dropGBs[i] = newGB;
        }
    }

    private void Update()
    {
        if (drops.Count == 0) return;
        float time = Time.deltaTime;
        foreach(var drop in drops.heap)
        {
            drop.time += time;
        }

        // 将超时的掉落物删除
        if(drops.Peek().time >= maxTime)
        {
            Destroy(drops.Peek().id.ToString());
        }
    }

    // 获取掉落物
    public static Item GetDrop(string name)
    {
        int id = int.Parse(name);
        Drop drop = dropList[id];
        if (!dropGBs[drop.id].activeSelf)
        {
            Debug.Log("不存在该物品");
            return null;
        }
        return drop.item;
    }

    // 增加掉落物
    public static void AddDrop(Item item, Vector3 posi)
    {
        if(drops.Count >= maxDrop)
        {
            // 到达上限，删除堆顶
            Destroy(drops.Peek().id.ToString());
        }
        int id = GetGB();
        CreateDrop(item, id, false, posi);
        Drop drop = new(id, item);
        dropList[id] = drop;
        drops.Enqueue(drop);
    }

    // 删除掉落物
    public static void Destroy(string name)
    {
        int id = int.Parse(name);
        Drop drop = dropList[id];
        if (!drops.Remove(drop))
        {
            Debug.Log("不存在该物品");
            return;
        }
        dropGBs[drop.id].SetActive(false);
        CreateDrop(drop.item, drop.id, true, Vector3.zero);
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
    public static void CreateDrop(Item item, int id, bool flag, Vector3 posi)
    {
        dropGBs[id].transform.position = posi;
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
            dropGBs[id].GetComponent<CreateUI>().CreateBlockDrop(block, 0.3f, new Vector3(0.15f, -0.15f, -0.15f));
        }
    }
}
