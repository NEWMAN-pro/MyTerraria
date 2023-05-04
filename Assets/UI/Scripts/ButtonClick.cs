using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class ButtonClick : MonoBehaviour, IPointerClickHandler
{

    public GameObject condition;
    public Make make;

    private void Awake()
    {
        condition = GameObject.Find("Condition");
        make = GameObject.Find("Make").GetComponent<Make>();
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
            Debug.Log("Right click!");
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 鼠标左键被点击
            // 获取合成材料队列
            List<ItemMaterial> materials = make.synthesis[(byte)ID].materials;
            condition.GetComponent<Text>().text = "合成条件： 无";
            byte cnt = 0;
            foreach (var pair in materials)
            {
                Item item = make.synthesis[pair.id];
                if (cnt >= make.materialCount)
                {
                    GameObject itemBox = Instantiate(make.itemPrefab, this.transform.parent.parent.parent.parent.GetChild(4));
                    itemBox.name = "item " + cnt.ToString();
                    make.materialCount++;
                }
                make.gameObject.transform.GetChild(4).GetChild(cnt).gameObject.SetActive(true);
                make.CreateUI(item, cnt, this.transform.parent.parent.parent.parent.GetChild(4));
                cnt++;
            }
            for(int i = cnt; i < make.materialCount; i++)
            {
                make.gameObject.transform.GetChild(4).GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}