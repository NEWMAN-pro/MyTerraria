using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterList : MonoBehaviour
{
    // 怪物预制体
    GameObject slimPrefab;
    GameObject zombiePrefab;

    // 每类怪物对象的数量
    public static int Count = 30;

    // 怪物队列
    public static Dictionary<MonsterType, List<GameObject>> monsterList = new();

    private void Awake()
    {
        monsterList[MonsterType.Slim] = new();
        monsterList[MonsterType.Zombie] = new();
        slimPrefab = Resources.Load("Prefabs/Monsters/Slim") as GameObject;
        zombiePrefab = Resources.Load("Prefabs/Monsters/Zombie") as GameObject;
        for (int i = 0; i < Count; i++)
        {
            GameObject slim = Instantiate(slimPrefab, Vector3.zero, Quaternion.identity);
            slim.SetActive(false);
            slim.name = "Slim";
            slim.transform.SetParent(this.transform);
            monsterList[MonsterType.Slim].Add(slim);
            GameObject zombie = Instantiate(zombiePrefab, Vector3.zero, Quaternion.identity);
            zombie.SetActive(false);
            zombie.name = "Zombie";
            zombie.transform.SetParent(this.transform);
            monsterList[MonsterType.Zombie].Add(zombie);
        }
    }

    // 激活一个怪
    public static GameObject ActivateMonster(MonsterType flag)
    {
        for (int i = 0; i < Count; i++)
        {
            if (!monsterList[flag][i].activeSelf)
            {
                // 找到处于未激活状态
                monsterList[flag][i].SetActive(true);
                return monsterList[flag][i];
            }
        }
        Debug.Log("该类型怪物已经满了");
        return null;
    }
}
