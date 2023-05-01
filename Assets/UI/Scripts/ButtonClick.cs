using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class ButtonClick : MonoBehaviour, IPointerClickHandler
{
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
        else if(eventData.button == PointerEventData.InputButton.Left)
        {
            // 鼠标左键被点击
            Debug.Log("Left click!");
        }
    }
}