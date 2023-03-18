using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    // �浵����
    public static string key = "";
    // ��ɫ����
    public static string playerName = "";
    // �Ƿ�������Ϸ
    public static bool flag = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ��תҳ��
    public static void ToScenes()
    {
        SceneManager.LoadScene(1);
    }

    // ����Ϸ
    public void NewGame()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(2).gameObject.SetActive(true);
    }

    // ������Ϸ
    public void Continue()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.transform.GetChild(1).gameObject.SetActive(true);
    }

    // ��������
    public void CreateWorld()
    {
        playerName = GameObject.Find("InputPlayerName").GetComponent<InputField>().text;
        key = GameObject.Find("InputWorldName").GetComponent<InputField>().text + ".txt";
        flag = true;
        ToScenes();
    }
}
