using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class ButtonClick : MonoBehaviour, IPointerClickHandler
{

    public GameObject condition;
    public Make make;
    public Backpack backpack;

    private void Awake()
    {
        condition = GameObject.Find("Condition");
        make = GameObject.Find("Make").GetComponent<Make>();
        backpack = GameObject.Find("Backpack").GetComponent<Backpack>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 获取点击物品的ID
        GameObject button = eventData.pointerPress;
        string pattern = @"\bitem\s+(\d+)\b";
        Regex regex = new(pattern);
        MatchCollection matches = regex.Matches(button.name);
        int ID = int.Parse(matches[0].Groups[1].Value);

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 鼠标右键被点击
            Dictionary<byte, Item> itemmaterials = new();
            // 获取合成材料队列
            Item item = make.synthesis[(byte)ID].Clone();
            List<ItemMaterial> materials = item.materials;
            if (materials.Count == 0) return;
            foreach(var material in materials)
            {
                var query = from pair in Backpack.items
                            where pair.Value != null && pair.Value.ID == material.id && pair.Value.count >= material.y
                            select pair;
                if (query.Any())
                {
                    // 如果不为空
                    itemmaterials.Add(query.First().Key, query.First().Value);
                    itemmaterials[query.First().Key].count -= material.y;
                }
                else
                {
                    Debug.Log("材料不足");
                    break;
                }
            }
            if(itemmaterials.Count == materials.Count)
            {
                // 所有材料都充足
                foreach(var pair in itemmaterials)
                {
                    // 扣除材料
                    if (pair.Value.count == 0)
                    {
                        backpack.SetItem(pair.Key, null);
                    }
                    else
                    {
                        backpack.SetItem(pair.Key, pair.Value);
                    }
                }
                // 获得合成物品
                if(materials[0].x != -1) item.count = materials[0].x;
                backpack.Storage(item);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 鼠标左键被点击
            // 修改标记颜色
            if(make.itemFlag != -1) make.SetColor(make.itemFlag, false);
            make.SetColor(ID, true);
            make.itemFlag = ID;
            // 获取合成材料队列
            List<ItemMaterial> materials = make.synthesis[(byte)ID].materials;
            condition.GetComponent<Text>().text = "合成条件： 无";
            byte cnt = 0;
            foreach (var pair in materials)
            {
                Item item = make.synthesis[pair.id].Clone();
                if (cnt >= make.materialCount)
                {
                    GameObject itemBox = Instantiate(make.itemPrefab, make.transform.GetChild(4));
                    itemBox.name = "item " + cnt.ToString();
                    make.materialCount++;
                }
                make.gameObject.transform.GetChild(4).GetChild(cnt).gameObject.SetActive(true);
                item.count = pair.y;
                make.CreateUI(item, cnt, make.transform.GetChild(4));
                cnt++;
            }
            for(int i = cnt; i < make.materialCount; i++)
            {
                make.transform.GetChild(4).GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}