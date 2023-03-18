using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class SelectArchive : MonoBehaviour
{
    // �浵�ļ�·��
    public string path;
    // �浵�ļ����ƶ���
    public List<string> archives = new();
    // �浵��Ԥ����
    public GameObject archivePrefab;

    private void Awake()
    {
        path = Application.dataPath + "/Save";
        archivePrefab = Resources.Load("Prefabs/Archive") as GameObject;
        // ��ʼ��ʱ�����ļ���
        Directory.CreateDirectory(Application.dataPath + "/Save");
    }

    // ������ʱ�������д浵�ļ���
    private void OnEnable()
    {
        Transform content = this.transform.GetChild(0).GetChild(0).GetChild(0);
        string[] files = Directory.GetFiles(path, "*.txt");
        foreach(string file in files)
        {
            archives.Add(Path.GetFileNameWithoutExtension(file));
            GameObject archive = Instantiate(archivePrefab, new Vector3(0, 25, 0), Quaternion.identity);
            archive.transform.SetParent(content);
            archive.GetComponent<RectTransform>().anchoredPosition = new(0, -150f - (archives.Count - 1) * 250f);
            archive.transform.GetChild(0).GetComponent<Text>().text = archives[^1];
            archive.name = archives[^1];
            // �󶨰�ť����¼�
            archive.GetComponent<Button>().onClick.AddListener(() =>
            {
                Archive();
            });
        }
        // �޸Ŀ��
        content.GetComponent<RectTransform>().sizeDelta = new(0f, Mathf.Max(1000f, archives.Count * 250f));
        content.GetComponent<RectTransform>().anchoredPosition = new(0, -content.GetComponent<RectTransform>().sizeDelta.y / 2 + 500);
    }

    // ѡ��浵
    public void Archive()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(name);
        StartUI.key = name + ".txt";
        StartUI.flag = false;
        StartUI.ToScenes();
    }
}
