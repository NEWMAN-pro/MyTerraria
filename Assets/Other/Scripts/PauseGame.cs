using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    // 对象列表
    GameObject[] gameObjects;
    // 脚本列表
    List<MonoBehaviour> mbList;

    // Start is called before the first frame update
    void Start()
    {
        // 遍历所有对象
        foreach(var gb in gameObjects)
        {
            // 获取对象上所有组件
            Component[] components = gb.GetComponents<Component>();

            foreach(var mb in components)
            {
                // 筛选脚本
                if(mb is MonoBehaviour)
                {
                    // 添加脚本
                    mbList.Add((MonoBehaviour)mb);
                }
            }
        }
    }

    public void OnPauseGame()
    {

    }
}
