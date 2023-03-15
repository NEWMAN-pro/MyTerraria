using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Box : MonoBehaviour
{
    // ��������
    public string boxName;
    // ������Կ
    public string key;
    // ��Ʒ����
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();
    // ��Ʒ����text
    public GameObject textPrefab;
    public GameObject text;
    // ����
    public Backpack backpack;

    // �������������
    public InputField inputField;

    private void OnEnable()
    {
        // ��ʾ�������ʱ���������µ�
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -765);
        this.transform.parent.GetChild(61).gameObject.SetActive(false);
        this.transform.parent.GetChild(62).gameObject.SetActive(false);
        foreach(var pair in items)
        {
            // ���»��Ʊ�����Ʒ
            CreateUI(pair.Value, pair.Key);
        }
        inputField.text = boxName;
        inputField.interactable = false;
    }

    private void Awake()
    {
        // ��Ʒ������ʼ��
        textPrefab = Resources.Load("Prefabs/ItemCount") as GameObject;
        for (byte i = 0; i < 50; i++)
        {
            items[i] = null;
            text = Instantiate(textPrefab, this.transform.GetChild(i));
            text.name = "ItemCount";
        }
        backpack = this.transform.parent.GetComponent<Backpack>();
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
            this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = "";
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
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
        }
        if (item.count != -1) this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = item.count.ToString();
    }

    // ����ѡ���
    public Item Select(int key, ref Item selectItem, ref RectTransform select, ref int flag)
    {
        key -= 60;
        Item item = GetItem((byte)key);
        if (Input.GetKey(KeyCode.LeftShift) && item != null && !item.flag)
        {
            // ����ǰ���LeftShift�������Ҹ���Ʒδ����ǣ��򽫸ø���Ʒ���뱳���������ø��ÿ�
            SetItem((byte)key, null);
            flag = 1;
            return item;
        }
        if (Input.GetKey(KeyCode.LeftControl) && item != null && !item.flag)
        {
            // ����ǰ���LeftControl�������Ҹ���Ʒδ����ǣ�����Ʒ��������Ͱ
            // ���������
            flag = 2;
            SetItem((byte)key, null);
            return item;
        }
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
        // ���ֵ������ȱȽ�type�ٱȽ�ID����С���󣩣��ٱȽ�key����С����
        var sortedItems_ = items.Where(i => i.Value != null)
           .OrderBy(i => i.Value.type).ThenBy(i => i.Value.ID).ThenBy(i => i.Key)
           .ToList();
        for (int i = 0; i < sortedItems_.Count; i++)
        {
            // ���кϲ�
            var pair = sortedItems_[i];
            if (i != sortedItems_.Count - 1)
            {
                var nextPair = sortedItems_[i + 1];
                if (pair.Value.type == nextPair.Value.type && pair.Value.ID == nextPair.Value.ID)
                {
                    // ������Ʒ��������
                    int ans = pair.Value.count + nextPair.Value.count;
                    if (ans <= 64)
                    {
                        pair.Value.count = ans;
                        // ����Ϊ0��ɾ����Ʒ
                        sortedItems_[i + 1].Value.count = 0;
                        SetItem(nextPair.Key, null);
                    }
                    else
                    {
                        pair.Value.count = 64;
                        sortedItems_[i + 1].Value.count = ans - 64;
                    }
                }
            }
        }
        // ���ֵ������ȱȽ�type�ٱȽ�ID
        Dictionary<byte, Item> sortedItems = sortedItems_.Where(i => i.Value != null && i.Value.count != 0)
            .OrderBy(i => i.Value.type).ThenBy(i => i.Value.ID)
            .ToDictionary(i => i.Key, i => i.Value);
        byte key = 0;
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

    // ������Ʒ
    public bool Storage(Item item)
    {
        byte key;
        // ����Ѱ����ͬ����Ʒ
        key = items.FirstOrDefault(x => x.Value != null && x.Value.type == item.type && x.Value.ID == item.ID).Key;
        Item item1 = GetItem(key);
        if (item1 != null && item1.type == item.type && item1.ID == item.ID && item1.count != -1)
        {
            // ����ҵ���ͬ����Ʒ��������Ʒ���Ժϲ�
            int ans = item1.count + item.count;
            if (ans <= 64)
            {
                // ��ƴ������δ�����ޣ���ϲ�
                item1.count = ans;
                SetItem(key, item1);
                return true;
            }
            else
            {
                // �����ޣ����´���һ����Ʒ
                item1.count = 64;
                item.count = ans - 64;
                SetItem(key, item1);
            }
        }
        // �ҵ���һ��Ϊ�յĿո�
        key = items.FirstOrDefault(x => x.Value == null).Key;
        if(GetItem(key) != null)
        {
            // �����ǰ����û�ҵ��ո�
            // ���ӷ�����
            return false;
        }
        SetItem(key, item);
        return true;
    }

    // ������
    public void SetName()
    {
        inputField.interactable = true;
    }

    // �������
    public void EndInput()
    {
        boxName = inputField.text;
        inputField.interactable = false;
    }

    // ǿ��ȫ��
    public void Snatch()
    {

        foreach(var pair in items)
        {
            if(pair.Value != null) backpack.Storage(pair.Value);
        }
        for(byte i = 0; i < 50; i++)
        {
            SetItem(i, null);
        }
    }

    // ����
    public void Replenishment()
    {
        // �ҵ������뱦������ͬ����Ʒ����δ�����
        var items1 = items.Where(i => i.Value != null && !i.Value.flag).ToDictionary(i => i.Key, i => i.Value);
        // ����Ͱ����Ʒ������ƥ��
        var items2 = backpack.items.Where(i => i.Key != 50).ToDictionary(i => i.Key, i => i.Value);
        var commonItems = items1.Values.Intersect(items2.Values, new ItemEqualityComparer()).ToList();
        var commonItemsWithKeys = items1.Where(pair => commonItems.Contains(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
        foreach (var pair in commonItemsWithKeys)
        {
            backpack.Storage(pair.Value);
            SetItem(pair.Key, null);
        }
    }

    private void OnDisable()
    {
        // ���ر������ʱ���������λ
        this.transform.parent.GetChild(50).GetComponent<RectTransform>().anchoredPosition = new Vector2(352, -262.57f);
        this.transform.parent.GetChild(61).gameObject.SetActive(true);
        this.transform.parent.GetChild(62).gameObject.SetActive(true);
        // ����ʱ��ն���
        BoxList.SetBox(key, items, boxName);
        foreach(var pair in items)
        {
            CreateUI(null, pair.Key);
        }
    }
}
