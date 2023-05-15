using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State : MonoBehaviour
{
    // 血条预制体
    public GameObject HPPrefab;
    // 蓝条预制体
    public GameObject MPPrefab;

    // 血条队列
    List<GameObject> HPList = new();
    // 蓝条队列
    List<GameObject> MPList = new();

    // Start is called before the first frame update
    void Start()
    {
        HPPrefab = Resources.Load("Prefabs/HP") as GameObject;
        MPPrefab = Resources.Load("Prefabs/MP") as GameObject;
        for(int i = 0; i < 10; i++)
        {
            GameObject hp = Instantiate(HPPrefab, this.transform.GetChild(2));
            HPList.Add(hp);
            GameObject mp = Instantiate(MPPrefab, this.transform.GetChild(3));
            MPList.Add(mp);

        }
    }

    // 绘制血条或蓝条
    public void CreateUI(int num, int maxNum, bool flag)
    {
        // 计算爱心个数
        int size = (int)Mathf.Ceil(num / (maxNum / 10f));
        for(int i = 9; i >= size; i--)
        {
            if (flag)
            {
                HPList[i].SetActive(false);
            }
            else
            {
                MPList[i].SetActive(false);
            }
        }
        for(int i = 0; i < size; i++)
        {
            if (flag)
            {
                HPList[i].SetActive(true);
            }
            else
            {
                MPList[i].SetActive(true);
            }
        }
        this.transform.GetChild(flag ? 0 : 1).GetComponent<Text>().text = num.ToString() + " / " + maxNum.ToString();
    }
}
