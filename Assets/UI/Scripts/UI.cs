using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    bool flag = true;
    public bool boxFlag = true;

    // Start is called before the first frame update
    void Start()
    {
        // 隐藏鼠标，并将鼠标固定在中心
        Cursor.lockState = CursorLockMode.Locked;
        // 初始隐藏UI
        this.transform.GetChild(1).gameObject.SetActive(false);
        this.transform.GetChild(2).gameObject.SetActive(false);
        this.transform.GetChild(3).gameObject.SetActive(false);
        this.transform.GetChild(4).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.transform.GetChild(5).gameObject.activeSelf && Input.GetKeyUp(KeyCode.Escape))
        {
            OpenUI();
        }
    }

    // 打开宝箱界面
    public void OpenBox(string key)
    {
        GameObject box = this.transform.GetChild(1).GetChild(60).gameObject;
        box.GetComponent<Box>().items = BoxList.GetBox(key, out box.GetComponent<Box>().boxName);
        box.GetComponent<Box>().key = key;
        box.SetActive(boxFlag);
        if (flag)
        {
            OpenUI();
        }
        boxFlag = !boxFlag;
    }

    // 打开UI，不打开宝箱界面
    public void OpenUI()
    {
        this.transform.GetChild(1).gameObject.SetActive(flag);
        this.transform.GetChild(2).gameObject.SetActive(flag);
        this.transform.GetChild(3).gameObject.SetActive(flag);
        this.transform.GetChild(4).gameObject.SetActive(flag);
        this.transform.GetChild(7).gameObject.SetActive(!flag);
        // 控制鼠标显隐
        if (!flag)
        {
            if (!boxFlag)
            {
                this.transform.GetChild(1).GetChild(60).gameObject.SetActive(boxFlag);
                boxFlag = !boxFlag;
            }
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // 将鼠标限制在游戏窗口内
            Cursor.lockState = CursorLockMode.Confined;
        }
        flag = !flag;
        if (!flag)
        {
            GameObject.Find("Map").GetComponent<PauseGameAll>().OnPauseGame();
        }
        else
        {
            GameObject.Find("Map").GetComponent<PauseGameAll>().UnPauseGame();
        }
    }

    // 打开设置界面
    public void Set()
    {
        for(int i = 0; i < this.transform.childCount - 1; i++)
        {
            if(i == 5)
            {
                this.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    // 继续游戏
    public void Continue()
    {
        this.transform.GetChild(5).gameObject.SetActive(false);
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(6).gameObject.SetActive(true);
        OpenUI();
    }

    // 返回主界面
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
