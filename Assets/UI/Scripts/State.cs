using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State : MonoBehaviour
{
    // Ѫ��Ԥ����
    public GameObject HPPrefab;
    // ����Ԥ����
    public GameObject MPPrefab;

    // Start is called before the first frame update
    void Start()
    {
        HPPrefab = Resources.Load("Prefabs/HP") as GameObject;
        MPPrefab = Resources.Load("Prefabs/MP") as GameObject;
    }

    // ����Ѫ��������
    public void CreateUI(int num, int maxNum, bool flag)
    {
        GameObject prefab = flag ? HPPrefab : MPPrefab;
        // ���㰮�ĸ���
        int size = (int)Mathf.Ceil(num / 10);
        for(int i = 0; i < size; i++)
        {
            GameObject pp = Instantiate(prefab);
            pp.transform.SetParent(this.transform);
            pp.GetComponent<RectTransform>().anchoredPosition = new Vector2(-270f + i * 60, flag ? -30 : 30);
        }
        this.transform.GetChild(flag ? 0 : 1).GetComponent<Text>().text = num.ToString() + " / " + maxNum.ToString();
    }
}
