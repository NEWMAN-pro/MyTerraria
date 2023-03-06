using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Box : MonoBehaviour
{
    // ��Ʒ����
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();

    private void OnEnable()
    {
        // ��ʾ�������ʱ���������µ�
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -765);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ��ȡ��Ʒ
    public Item GetItem(byte key)
    {
        return items.ContainsKey(key) ? items[key] : null;
    }

    // �޸���Ʒ
    public void SetItem(byte key, Item item)
    {
        items[key] = item;
        CreateUI(item, key);
    }

    // ����
    public void CreateUI(Item item, byte key)
    {
        if (item == null)
        {
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlank();
            return;
        }
        if (item.type == Type.Block)
        {
            Block block = BlockList.GetBlock(item.ID);
            if (block == null)
            {
                Debug.Log("����ƷΪ��ba");
                return;
            }
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 50, new Vector3(0, -0.9f, -0.01f));
        }
    }

    // ����ѡ���
    public Item Select(int key, ref Item selectItem, ref RectTransform select)
    {
        key -= 60;
        Item item = GetItem((byte)key);
        if (selectItem != null)
        {
            selectItem.flag = false;
            SetItem((byte)key, selectItem);
            if (item == null)
            {
                // ����ø�Ϊ�գ����ÿ�ѡ���
                selectItem = null;
                select.gameObject.SetActive(false);
            }
            else
            {
                // �������ѡ������Ʒ
                selectItem = item;
                return item;
            }
        }
        else
        {
            // ѡ���û����Ʒ�����ȡ�ø���Ʒ
            if (item == null)
            {
                return null;
            }
            select.gameObject.SetActive(true);
            selectItem = item;
            SetItem((byte)key, null);
            return item;
        }
        return null;
    }

    // һ������
    public void Neaten()
    {
        // ���ֵ������ȱȽ�type�ٱȽ�ID
        Dictionary<byte, Item> sortedItems = items.Where(i => i.Value != null)
            .OrderBy(i => i.Value.type).ThenBy(i => i.Value.ID)
            .ToDictionary(i => i.Key, i => i.Value);
        byte key = 10;
        foreach (var pair in sortedItems)
        {
            // �����
            SetItem(pair.Key, null);
        }
        foreach (var pair in sortedItems)
        {
            while (GetItem(key) != null)
            {
                // �ҵ���Ϊ�յı�����
                key++;
            }
            // �����»���
            SetItem(key++, pair.Value);
        }
    }

    private void OnDisable()
    {
        // ���ر������ʱ���������λ
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -262.57f);
    }
}
