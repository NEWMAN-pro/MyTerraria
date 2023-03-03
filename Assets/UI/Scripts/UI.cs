using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    bool flag = true;
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
            this.transform.GetChild(1).gameObject.SetActive(flag);
            // �����������
            if (!flag)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            flag = !flag;
        }
    }
}
