using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DanmuType
{
    NormalBall,
    Arrow
}

public class DanmuList : MonoBehaviour
{
    // 弹幕队列
    public static Dictionary<DanmuType, List<GameObject>> danmuList = new();

    // 最多弹幕数量
    public static int Count = 100;

    private void Awake()
    {
        danmuList[DanmuType.NormalBall] = new();
        danmuList[DanmuType.Arrow] = new();

        GameObject normalBallPrefab = Resources.Load("Prefabs/Weapons/NormalBall") as GameObject;
        for(int i = 0; i  < Count; i++)
        {
            GameObject normalBall = Instantiate(normalBallPrefab);
            normalBall.SetActive(false);
            normalBall.name = "NormalBall";
            normalBall.transform.SetParent(this.transform);
            danmuList[DanmuType.NormalBall].Add(normalBall);
        }
    }

    public static GameObject ActivateDanmu(DanmuType danmu)
    {
        for(int i = 0; i < Count; i++)
        {
            if (!danmuList[danmu][i].activeSelf)
            {
                // 找到未激活的弹幕对象
                danmuList[danmu][i].SetActive(true);
                return danmuList[danmu][i];
            }
        }
        Debug.Log("弹幕已经达到上限");
        return null;
    }
}
