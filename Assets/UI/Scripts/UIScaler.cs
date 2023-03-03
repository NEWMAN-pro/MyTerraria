using UnityEngine;
using UnityEngine.UI;

public class UIScaler : MonoBehaviour
{
    public Vector2 posi;
    public float width = 1920;
    public float height = 1080;
    private float flag;

    private RectTransform rectTransform;

    void Start()
    {
        // 获取目标UI的RectTransform组件
        rectTransform = GetComponent<RectTransform>();
        posi = rectTransform.anchoredPosition;
        flag = posi.y > 0 ? 1f : -1f;
    }

    void Update()
    {
        // UI坐标随屏幕长宽变化而变化
        Vector2 newPosi = new Vector2(posi.x, posi.y - flag * (height - Screen.height / (float)Screen.width * width) / 2.0f);
        this.rectTransform.anchoredPosition = newPosi;
    }
}
