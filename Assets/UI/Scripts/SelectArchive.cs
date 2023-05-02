using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class SelectArchive : MonoBehaviour
{
    // 存档文件路径
    public string path;
    // 存档文件名称队列
    public List<string> archives = new();
    // 存档框预制体
    public GameObject archivePrefab;

    private void Awake()
    {
        path = Application.dataPath + "/Save";
        archivePrefab = Resources.Load("Prefabs/Archive") as GameObject;
        // 初始化时创建文件夹
        Directory.CreateDirectory(Application.dataPath + "/Save");
    }

    // 被激活时搜索所有存档文件名
    private void OnEnable()
    {
        archives.Clear();
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
            // 绑定按钮点击事件
            archive.GetComponent<Button>().onClick.AddListener(() =>
            {
                Archive();
            });
        }
        // 修改框高
        content.GetComponent<RectTransform>().sizeDelta = new(0f, Mathf.Max(1000f, archives.Count * 250f));
        content.GetComponent<RectTransform>().anchoredPosition = new(0, -content.GetComponent<RectTransform>().sizeDelta.y / 2 + 500);
    }

    // 选择存档
    public void Archive()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(name);
        StartUI.key = name + ".txt";
        StartUI.flag = false;
        StartUI.ToScenes();
    }
}
