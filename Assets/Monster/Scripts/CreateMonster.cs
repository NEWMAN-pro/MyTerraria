using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;

public class CreateMonster : MonoBehaviour
{
    // 怪物最大数量
    public int maxCount = 30;
    // 怪物计数器
    public static int count = 0;
    // 怪物生成间隔
    public float intervalTime = 3;
    // 是否在生成
    public bool createFlag = false;

    GameObject monster;

    private void Awake()
    {
    }

    private void Update()
    {
        if(!createFlag && count < maxCount)
        {
            StartCreateMonster();
        }
    }

    // 开启怪物生成
    public void StartCreateMonster()
    {
        createFlag = true;
        InvokeRepeating(nameof(Create), 5, intervalTime);
    }

    // 关闭怪物生成
    public void StopCreateMonster()
    {
        createFlag = false;
        CancelInvoke(); 
    }

    // 生成一只怪物
    public void Create()
    {
        Vector3 createPosi = this.transform.position;
        // 确定平面位置
        float x = Random.Range(0f, 1000f) % 8f + 8f;
        float z = Random.Range(0f, 1000f) % 8f + 8f;
        createPosi.x += x * (Random.Range(0, 100) % 2 == 0 ? 1 : -1);
        createPosi.z += z * (Random.Range(0, 100) % 2 == 0 ? 1 : -1);
        // 确定生成高度
        createPosi.y = 16;// 暂时
        if(Random.Range(0, 100) % 2 == 0)
        {
            // 生成Slim
            monster = MonsterList.ActivateMonster(MonsterType.Slim);
        }
        else
        {
            // 生成Zombie
            monster = MonsterList.ActivateMonster(MonsterType.Zombie);
        }
        monster.transform.position = createPosi;
        count++;
        if (count >= maxCount)
        {
            // 达到数量上限，停止生成怪物
            StopCreateMonster();
        }
    }
}
