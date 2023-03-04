using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Backpack : MonoBehaviour
{
    // ��Ʒ����
    public Dictionary<byte, Item> items = new Dictionary<byte, Item>();
    // ��Ʒ��
    public Inventory inventory;
    // ѡ���
    public RectTransform select;
    // ��ǰѡ�����Ʒ
    public Item selectItem;

    private void Awake()
    {
        Item item_1 = new Item();
        item_1.type = 0;
        item_1.ID = 3;
        items.Add(0, item_1);
        inventory.SetItem(1, item_1);
        CreateUI(item_1, 0, false);
        Item item_2 = new Item();
        item_2.type = 0;
        item_2.ID = 1;
        items.Add(1, item_2);
        inventory.SetItem(2, item_2);
        CreateUI(item_2, 1, false);
        Item item_3 = new Item();
        item_3.type = 0;
        item_3.ID = 2;
        items.Add(2, item_3);
        inventory.SetItem(3, item_3);
        CreateUI(item_3, 2, false);
        Item item_4 = new Item();
        item_4.type = 0;
        item_4.ID = 4;
        items.Add(3, item_4);
        inventory.SetItem(4, item_4);
        CreateUI(item_4, 3, false);
        Item item_5 = new Item();
        item_5.type = 0;
        item_5.ID = 5;
        items.Add(4, item_5);
        inventory.SetItem(5, item_5);
        CreateUI(item_5, 4, false);
        Item item_6 = new Item();
        item_6.type = 0;
        item_6.ID = 6;
        items.Add(5, item_6);
        inventory.SetItem(6, item_6);
        CreateUI(item_6, 5, false);
        Item item_7 = new Item();
        item_7.type = 0;
        item_7.ID = 7;
        items.Add(6, item_7);
        inventory.SetItem(7, item_7);
        CreateUI(item_7, 6, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
                select.GetComponent<CreateUI>().CreateBlockUI(block, true, 50, new Vector3(0, -0.9f, -0.01f));
            }
            else
            {
                this.transform.GetChild(key).GetComponent<CreateUI>().CreateBlockUI(block, true, 50, new Vector3(0, -0.9f, -0.01f));
            }
        }
    }

    // �����޸Ŀ�
    public void Select(int key)
    {
        Item item = GetItem((byte)key);
        if (selectItem != null)
        {
            // ��ѡ����е�������Ʒ�ŵ��ø�
            SetItem((byte)key, selectItem);
            if(item == null)
            {
                // ����ø�Ϊ�գ����ÿ�ѡ���
                selectItem = null;
                //select.SetActive(false);
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
            //select.SetActive(true);
            select.gameObject.SetActive(true);
            CreateUI(item, (byte)key, true);
            selectItem = item;
            SetItem((byte)key, null);
        }
    }

    // ����ʱ
    private void OnDisable()
    {
        if(selectItem != null)
        {
            // �����ѡ���δ��յ�������˳������ҵ������е�һ��Ϊ�յĸ��ӣ���ѡ����е�������벢���ѡ���
            byte key = items.FirstOrDefault(x => x.Value == null).Key;
            SetItem(key, selectItem);
            selectItem = null;
            select.gameObject.SetActive(false);
        }
    }
}
