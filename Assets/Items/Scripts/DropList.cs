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
    // �����������
    public static Drop[] dropList = new Drop[maxDrop];
    // ���������
    public static PriorityQueue<Drop> drops = new();
    // ���������ʱ�� s
    public float maxTime = 60;

    private void Start()
    {
        GameObject gameObject = Resources.Load("Prefabs/Drop") as GameObject;
        // ��ʼ���������
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

        // ����ʱ�ĵ�����ɾ��
        if(drops.Peek().time >= maxTime)
        {
            Destroy(drops.Peek().id.ToString());
        }
    }

    // ��ȡ������
    public static Item GetDrop(string name)
    {
        int id = int.Parse(name);
        Drop drop = dropList[id];
        if (!dropGBs[drop.id].activeSelf)
        {
            Debug.Log("�����ڸ���Ʒ");
            return null;
        }
        return drop.item;
    }

    // ���ӵ�����
    public static void AddDrop(Item item, Vector3 posi)
    {
        if(drops.Count >= maxDrop)
        {
            // �������ޣ�ɾ���Ѷ�
            Destroy(drops.Peek().id.ToString());
        }
        int id = GetGB();
        CreateDrop(item, id, false, posi);
        Drop drop = new(id, item);
        dropList[id] = drop;
        drops.Enqueue(drop);
    }

    // ɾ��������
    public static void Destroy(string name)
    {
        int id = int.Parse(name);
        Drop drop = dropList[id];
        if (!drops.Remove(drop))
        {
            Debug.Log("�����ڸ���Ʒ");
            return;
        }
        dropGBs[drop.id].SetActive(false);
        CreateDrop(drop.item, drop.id, true, Vector3.zero);
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
    public static void CreateDrop(Item item, int id, bool flag, Vector3 posi)
    {
        dropGBs[id].transform.position = posi;
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
            dropGBs[id].GetComponent<CreateUI>().CreateBlockDrop(block, 0.3f, new Vector3(0.15f, -0.15f, -0.15f));
        }
    }
}
