using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    bool flag = true;
    bool boxFlag = true;
    // Start is called before the first frame update
    void Start()
    {
        // ������꣬�������̶�������
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            OpenUI();
        }
    }

    public void OpenBox(string key)
    {
        GameObject box = this.transform.GetChild(1).GetChild(60).gameObject;
        box.SetActive(boxFlag);
        box.GetComponent<Box>().items = BoxList.GetBox(key);
        box.GetComponent<Box>().key = key;
        if (flag)
        {
            OpenUI();
        }
        boxFlag = !boxFlag;
    }

    // ��UI�����򿪱������
    public void OpenUI()
    {
        this.transform.GetChild(1).gameObject.SetActive(flag);
        this.transform.GetChild(2).gameObject.SetActive(flag);
        this.transform.GetChild(3).gameObject.SetActive(flag);
        this.transform.GetChild(4).gameObject.SetActive(flag);
        // �����������
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
            Cursor.lockState = CursorLockMode.None;
        }
        flag = !flag;
        if (!flag)
        {
            GameObject.Find("Player").GetComponent<PlayController>().PauseGame();
        }
        else
        {
            GameObject.Find("Player").GetComponent<PlayController>().UnPauseGame();
        }
    }
}
