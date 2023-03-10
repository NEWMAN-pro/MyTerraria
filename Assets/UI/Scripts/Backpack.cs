using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Backpack : MonoBehaviour
{
    // ��Ʒ����
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();
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

    private void Awake()
    {
        for(byte i = 0; i < 60; i++)
        {
            items[i] = null;
        }

        Item item_1 = new Item();
        item_1.type = 0;
        item_1.ID = 3;
        //items.Add(0, item_1);
        items[0] = item_1;
        inventory.SetItem(1, item_1);
        CreateUI(item_1, 0, false);
        Item item_2 = new Item();
        item_2.type = 0;
        item_2.ID = 1;
        items[1] = item_2;
        inventory.SetItem(2, item_2);
        CreateUI(item_2, 1, false);
        Item item_3 = new Item();
        item_3.type = 0;
        item_3.ID = 2;
        items[2] = item_3;
        inventory.SetItem(3, item_3);
        CreateUI(item_3, 2, false);
        Item item_4 = new Item();
        item_4.type = 0;
        item_4.ID = 4;
        items[3] = item_4;
        inventory.SetItem(4, item_4);
        CreateUI(item_4, 3, false);
        Item item_5 = new Item();
        item_5.type = 0;
        item_5.ID = 5;
        items[4] = item_5;
        inventory.SetItem(5, item_5);
        CreateUI(item_5, 4, false);
        Item item_6 = new Item();
        item_6.type = 0;
        item_6.ID = 6;
        items[5] = item_6;
        inventory.SetItem(6, item_6);
        CreateUI(item_6, 5, false);
        Item item_7 = new Item();
        item_7.type = 0;
        item_7.ID = 7;
        items[6] = item_7;
        inventory.SetItem(7, item_7);
        CreateUI(item_7, 6, false);
        Item item_8 = new Item();
        item_8.type = 0;
        item_8.ID = 8;
        items[7] = item_8;
        inventory.SetItem(8, item_8);
        CreateUI(item_8, 7, false);
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
            }
            else
            {
                this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 40, new Vector3(0, -1f, -0.01f));
            }
        }
    }

    // ����ѡ���
    public void Select(int key)
    {
        if(key >= 60)
        {
            bool flag = false;
            Item item_ = box.Select(key, ref selectItem, ref select, ref flag);
            if(item_ != null)
            {
                if (flag)
                {
                    // ����Ǵ�����Ʒ
                    Storage(item_);
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
        if(Input.GetKey(KeyCode.LeftControl) && item != null)
        {
            // ����ǰ���LeftControl��������Ʒ��������Ͱ
            SetItem(50, item);
            // ���������
            SetItem((byte)key, null);
            return;
        }
        if(Input.GetKey(KeyCode.LeftShift) && item != null && !GameObject.Find("UI").GetComponent<UI>().boxFlag)
        {
            // ����ǰ���LeftShift�����Ҵ��˱�����棬�򽫸ø���Ʒ���뱳��
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
        // ���ֵ������ȱȽ�type�ٱȽ�ID����Ʒ�����������򣬱���ǵ���Ʒ����������
        Dictionary<byte, Item> sortedItems = items.Where(i => i.Value != null && i.Key > 9 && !i.Value.flag)
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
        // �ҵ���һ��Ϊ�յĿո�
        byte key = items.FirstOrDefault(x => x.Value == null).Key;
        Item item1 = GetItem(key);
        if (item1 != null)
        {
            // �����ǰ����û�ҵ��ո�
            // ����������
            return false;
        }
        SetItem(key, item);
        return true;
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
