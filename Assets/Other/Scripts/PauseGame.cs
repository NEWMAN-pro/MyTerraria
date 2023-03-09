using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    // �����б�
    GameObject[] gameObjects;
    // �ű��б�
    List<MonoBehaviour> mbList;

    // Start is called before the first frame update
    void Start()
    {
        // �������ж���
        foreach(var gb in gameObjects)
        {
            // ��ȡ�������������
            Component[] components = gb.GetComponents<Component>();

            foreach(var mb in components)
            {
                // ɸѡ�ű�
                if(mb is MonoBehaviour)
                {
                    // ��ӽű�
                    mbList.Add((MonoBehaviour)mb);
                }
            }
        }
    }

    public void OnPauseGame()
    {

    }
}
