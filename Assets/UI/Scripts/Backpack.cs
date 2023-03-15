using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Backpack : MonoBehaviour
{
    // ��Ʒ����
    public Dictionary<byte, Item> items = new();
    // ��Ʒ��
    public Inventory inventory;
    // ����
    public Box box;
    // ѡ���
    public RectTransform select;
    // ��ǰѡ�����Ʒ
    public Item selectItem;
    // δ��ǵ���ɫ
    public Color NotFlag;
    // ��ǵ���ɫ
    public Color FlagColor;
    // ��Ʒ����text
    public GameObject textPrefab;
    public GameObject text;

    private void Awake()
    {
        // ��Ʒ������ʼ��
        textPrefab = Resources.Load("Prefabs/ItemCount") as GameObject;
        for (byte i = 0; i < 60; i++)
        {
            items[i] = null;
            text = Instantiate(textPrefab, this.transform.GetChild(i));
            text.name = "ItemCount";
            if (i > 50 && i < 59)
            {
                // ����ǽ�Ҹ���ߵ�ҩ��������ı���λ��
                text.GetComponent<RectTransform>().anchoredPosition = new Vector3(20f, -15f, -0.4f);
            }
            if(i < 10)
            {
                text = Instantiate(textPrefab, inventory.transform.GetChild(i));
                text.name = "ItemCount";
            }
        }

        Item item_1 = new Item();
        item_1.type = 0;
        item_1.ID = 3;
        item_1.count = 60;
        items[0] = item_1;
        inventory.SetItem(1, item_1);
        CreateUI(item_1, 0, false);
        Item item_2 = new Item();
        item_2.type = 0;
        item_2.ID = 1;
        item_2.count = 1;
        items[1] = item_2;
        inventory.SetItem(2, item_2);
        CreateUI(item_2, 1, false);
        Item item_3 = new Item();
        item_3.type = 0;
        item_3.ID = 2;
        item_3.count = 1;
        items[2] = item_3;
        inventory.SetItem(3, item_3);
        CreateUI(item_3, 2, false);
        Item item_4 = new Item();
        item_4.type = 0;
        item_4.ID = 4;
        item_4.count = 1;
        items[3] = item_4;
        inventory.SetItem(4, item_4);
        CreateUI(item_4, 3, false);
        Item item_5 = new Item();
        item_5.type = 0;
        item_5.ID = 5;
        item_5.count = 1;
        items[4] = item_5;
        inventory.SetItem(5, item_5);
        CreateUI(item_5, 4, false);
        Item item_6 = new Item();
        item_6.type = 0;
        item_6.ID = 6;
        item_6.count = 1;
        items[5] = item_6;
        inventory.SetItem(6, item_6);
        CreateUI(item_6, 5, false);
        Item item_7 = new Item();
        item_7.type = 0;
        item_7.ID = 7;
        item_7.count = 1;
        items[6] = item_7;
        inventory.SetItem(7, item_7);
        CreateUI(item_7, 6, false);
        Item item_8 = new Item();
        item_8.type = 0;
        item_8.ID = 8;
        item_8.count = 10;
        items[7] = item_8;
        inventory.SetItem(8, item_8);
        CreateUI(item_8, 7, false);
        Item item_9 = new Item();
        item_9.type = 0;
        item_9.ID = 3;
        item_9.count = 10;
        items[8] = item_9;
        inventory.SetItem(9, item_9);
        CreateUI(item_9, 8, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(selectItem != null)
        {
            // ��ѡ���������ƶ�
            select.anchoredPosition = (((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2f) * 1920f / (float)Screen.width - this.transform.GetComponent<RectTransform>().anchoredPosition) * 1.25f;
        }

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
        CreateUI(item, key, false);
        if(key <= 9)
        {
            // ͬ��������Ʒ��
            byte newKey = (byte)(key + 1);
            if(newKey == 10)
            {
                newKey = 0;
            }
            inventory.SetItem(newKey, item);
        }
    }

    // ����
    public void CreateUI(Item item, byte key, bool selectFlag)
    {
        if(item == null)
        {
            this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlank();
            this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = "";
            if (selectFlag)
            {
                if (item.count != -1) select.GetChild(0).GetComponent<Text>().text = "";
            }
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
            if (selectFlag)
            {
                select.GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
                if (item.count != -1) select.GetChild(0).GetComponent<Text>().text = item.count.ToString();
                return;
            }
            else
            {
                this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
            }
        }
        if(item.count != -1) this.transform.GetChild(key).GetChild(0).GetComponent<Text>().text = item.count.ToString();
    }

    // ����ѡ���
    public void Select(int key)
    {
        if(key >= 60)
        {
            int flag = 0;
            Item item_ = box.Select(key, ref selectItem, ref select, ref flag);
            if(item_ != null)
            {
                if (flag == 1)
                {
                    // ����Ǵ�����Ʒ
                    Storage(item_);
                }
                else if(flag == 2)
                {
                    // �����ɾ����Ʒ
                    SetItem(50, item_);
                }
                else
                {
                    // ѡ����ȡ�����е���Ʒ
                    CreateUI(item_, (byte)key, true);
                }
            }
            return;
        }
        Item item = GetItem((byte)key);
        if (Input.GetKey(KeyCode.LeftAlt) && item != null && key < 50)
        {
            // ����ǰ���LeftAlt����������Ʒ
            item.flag = !item.flag;
            SetItem((byte)key, item);
            SetColor(key, item.flag);
            return;
        }
        if(Input.GetKey(KeyCode.LeftControl) && item != null && !item.flag && key != 50)
        {
            // ����ǰ���LeftControl�������Ҹ���Ʒδ����ǣ��Ҹø�������Ͱ������Ʒ��������Ͱ
            SetItem(50, item);
            // ���������
            SetItem((byte)key, null);
            return;
        }
        if(Input.GetKey(KeyCode.LeftShift) && item != null && !item.flag && !GameObject.Find("UI").GetComponent<UI>().boxFlag)
        {
            // ����ǰ���LeftShift�����Ҵ��˱�����棬���Ҹ���Ʒδ����ǣ��򽫸ø���Ʒ���뱳��
            if (box.Storage(item))
            {
                // �������ɹ������������
                SetItem((byte)key, null);
            }
            return;
        }
        if (selectItem != null)
        {
            // ��ѡ����е�������Ʒ�ŵ��ø�
            if(key == 50)
            {
                selectItem.flag = false;
            }
            if(key > 50 && key < 55)
            {
                if(selectItem.type != Type.Money)
                {
                    // �����Ʒ���Ͳ��ǽ�Ǯ���ܷ���Ǯ�Ҹ�
                    return;
                }
            }
            if(key > 55 && key < 59)
            {
                if(selectItem.type != Type.Ammo)
                {
                    // �����Ʒ���Ͳ��ǵ�ҩ���ܷ��뵯ҩ��
                    return;
                }
            }
            SetItem((byte)key, selectItem);
            SetColor(key, selectItem.flag);
            if(item == null || key == 50)
            {
                // ����ø�Ϊ�գ����ÿ�ѡ��򣬵�ѡ�е�������Ͱ������������Ʒ
                selectItem = null;
                select.gameObject.SetActive(false);
            }
            else
            {
                // �������ѡ������Ʒ
                CreateUI(item, (byte)key, true);
                selectItem = item;
            }
        }
        else
        {
            // ѡ���û����Ʒ�����ȡ�ø���Ʒ
            if (item == null)
            {
                return;
            }
            select.gameObject.SetActive(true);
            CreateUI(item, (byte)key, true);
            selectItem = item;
            SetItem((byte)key, null);
            SetColor(key, false);
        }
    }

    // һ������
    public void Neaten()
    {
        // ���ֵ������ȱȽ�type�ٱȽ�ID����С���󣩣��ٱȽ�flag��true��ǰ�����ٱȽ�key����С����
        var sortedItems_ = items.Where(i => i.Value != null)
           .OrderBy(i => i.Value.type).ThenBy(i => i.Value.ID).ThenByDescending(i => i.Value.flag).ThenBy(i => i.Key)
           .ToList();
        for(int i = 0; i < sortedItems_.Count; i++)
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
            // ����Ʒ���뱻��ǵ���Ʒ���»���
            if(pair.Key < 10 || pair.Value.flag)
            {
                if(pair.Value.count == 0)
                {
                    // ����Ϊ0��������
                    continue;
                }
                SetItem(pair.Key, pair.Value);
            }
        }
        // ����Ʒ���뱻��ǵ���Ʒ�Լ�����Ϊ0����Ʒ�޳�
        var sortedItems = sortedItems_.Where(i => i.Value != null && i.Value.count != 0 && i.Key > 9 && !i.Value.flag)
            .ToDictionary(i => i.Key, i => i.Value);
        byte key = 10;
        foreach (var pair in sortedItems)
        {
            // �����
            SetItem(pair.Key, null);
        }
        foreach (var pair in sortedItems)
        {
            while(GetItem(key) != null)
            {
                // �ҵ���Ϊ�յı�����
                key++;
            }
            // �����»���
            SetItem(key++, pair.Value);
        }
    }

    // ������Ʒ����ɫ
    public void SetColor(int key, bool flag)
    {
        this.transform.GetChild(key).GetComponent<Image>().color = flag ? FlagColor : NotFlag;
    }

    // ������Ʒ
    public bool Storage(Item item)
    {
        byte key;
        // ����Ѱ����ͬ����Ʒ
        key = items.FirstOrDefault(x => x.Value != null && x.Value.type == item.type && x.Value.ID == item.ID).Key;
        Item item1 = GetItem(key);
        if(item1 != null && item1.type == item.type && item1.ID == item.ID && item1.count != -1)
        {
            // ����ҵ���ͬ����Ʒ��������Ʒ���Ժϲ�
            int ans = item1.count + item.count;
            if(ans <= 64)
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
        if (GetItem(key) != null)
        {
            // �����ǰ����û�ҵ��ո�
            // ����������
            return false;
        }
        SetItem(key, item);
        return true;
    }

    // ���ȫ��
    public void Deposit()
    {
        var items_ = items.Where(i => i.Value != null && i.Key > 9 && !i.Value.flag).ToDictionary(i => i.Key, i => i.Value);
        foreach(var pair in items_)
        {
            box.Storage(pair.Value);
        }
        foreach(var pair in items_)
        {
            SetItem(pair.Key, null);
        }
    }

    // ���ٶѵ�
    public void Stack()
    {
        // �ҵ������뱦������ͬ����Ʒ����δ�����
        // ����Ͱ����Ʒ������ƥ��
        var items_ = items.Where(i => i.Key != 50 && i.Value != null && !i.Value.flag).ToDictionary(i => i.Key, i => i.Value);
        var commonItems = items_.Values.Intersect(box.items.Values, new ItemEqualityComparer()).ToList();
        var commonItemsWithKeys = items_.Where(pair => commonItems.Contains(pair.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
        foreach (var pair in commonItemsWithKeys)
        {
            box.Storage(pair.Value);
            SetItem(pair.Key, null);
        }
    }

    // �ű�����ʱ
    private void OnDisable()
    {
        if(selectItem != null)
        {
            // �����ѡ���δ��յ�������˳������ҵ������е�һ��Ϊ�յĸ��ӣ���ѡ����е�������벢���ѡ���
            Storage(selectItem);
            selectItem = null;
            select.gameObject.SetActive(false);
        }
    }
}

// �Զ���Ƚ���
public class ItemEqualityComparer : IEqualityComparer<Item>
{
    public bool Equals(Item x, Item y)
    {
        if (x == null || y == null)
        {
            return false;
        }
        else
        {
            return x.ID == y.ID && x.type == y.type;
        }
    }

    public int GetHashCode(Item obj)
    {
        return obj.ID.GetHashCode() ^ obj.type.GetHashCode();
    }
}
