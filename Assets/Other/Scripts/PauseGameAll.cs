using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameAll : MonoBehaviour
{
    // ��ͣ�ű�����
    public List<PauseGame> pgList = new();

    // �����ͣ�ű�
    public void Add(PauseGame pg)
    {
        pgList.Add(pg);
    }

    // ��ͣ���нű�
    public void OnPauseGame()
    {
        //Time.timeScale = 0f;
        foreach(var pg in pgList)
        {
            pg.OnPauseGame();
        }
    }

    // ������нű���ͣ
    public void UnPauseGame()
    {
        //Time.timeScale = 1f;
        foreach (var pg in pgList)
        {
            pg.UnPauseGame();
        }
    }
}
