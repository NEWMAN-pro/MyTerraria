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
        if (Input.GetKeyUp(KeyCode.L))
        {
            this.transform.GetChild(1).GetChild(60).gameObject.SetActive(boxFlag);
            if (flag)
            {
                OpenUI();
            }
            boxFlag = !boxFlag;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            OpenUI();
        }
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
    }
}
