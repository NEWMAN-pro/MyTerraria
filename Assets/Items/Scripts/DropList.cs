using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;

public class DropList : MonoBehaviour
{
    // ����������
    public static int maxDrop = 1000;
    // ������������
    public static GameObject[] dropGBs = new GameObject[maxDrop];
    // ���������
    public static PriorityQueue<Drop> drops;
    // ���������ʱ�� s
    public float maxTime = 60;

    private void Start()
    {
        GameObject gameObject = Resources.Load("Prefabs/Drop") as GameObject;
        // ��ʼ���������
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

        // ����ʱ�ĵ�����ɾ��
        while(drops.Peek().time >= maxTime)
        {
            drops.Dequeue();
        }
    }

    // ���ӵ�����
    public static void AddDrop(Item item)
    {
        if(drops.Count >= maxDrop)
        {
            // �������ޣ�ɾ���Ѷ�
            drops.Dequeue();
        }
        int id = GetGB();
        CreateDrop(item, id, false);
        Drop drop = new(id, item);
        drops.Enqueue(drop);
    }

    // ɾ��������
    public static void Destroy(Drop drop)
    {
        if (!drops.Remove(drop))
        {
            Debug.Log("�����ڸ���Ʒ");
            return;
        }
        dropGBs[drop.id].SetActive(false);
        CreateDrop(drop.item, drop.id, true);
    }

    // �ҵ�������е�һ��Ϊ�յ�ֵ
    public static int GetGB()
    {
        for(int i = 0; i < maxDrop; i++)
        {
            if (!dropGBs[i].activeSelf)
            {
                dropGBs[i].SetActive(true);
                // �ҵ�����
                return i;
            }
        }
        // û�з���-1
        return -1;
    }

    // ���Ƶ�����
    public static void CreateDrop(Item item, int id, bool flag)
    {
        if (flag)
        {
            // �����ɾ��������ͼ�ÿ�
            dropGBs[id].GetComponent<CreateUI>().CreateBlank();
            return;
        }
        if(item.type == Type.Block)
        {
            // ����Ƿ���
            Block block = BlockList.GetBlock(item.ID);
            dropGBs[id].GetComponent<CreateUI>().CreateBlockDrop(block, 1);
        }
    }
}
