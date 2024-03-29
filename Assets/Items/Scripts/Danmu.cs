﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Danmu : MonoBehaviour
{
    // 存在时间
    float time;
    // 最大存在时间
    float MaxTime = 20f;
    // 速度
    float speed = 7f;
    // 伤害
    public int damge;
    // 弹幕类型
    public DanmuType danmuType;

    private void Awake()
    {
        time = 0f;
        damge = 0;
    }

    private void OnEnable()
    {
        time = 0f;
        damge = 0;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if(time > MaxTime)
        {
            // 超过时限，销毁
            Dectory();
            return;
        }
        if (danmuType == DanmuType.NormalBall)
        {
            this.transform.position += speed * Time.deltaTime * transform.forward;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 忽略玩家
        int layerMask = (1 << LayerMask.NameToLayer("First")) | (1 << LayerMask.NameToLayer("Third"));
        layerMask = ~layerMask;
        if((layerMask & (1 << other.gameObject.layer)) == 0)
        {
            // 如果和玩家层级一样，则忽略
            return;
        }
        // 碰到物体时
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            // 如果时碰到怪物，则扣除怪物血量
            other.gameObject.GetComponent<Monster>().Hit(damge, Vector3.zero, 0);
        }
        // 销毁弹幕
        Dectory();
    }

    public void Dectory()
    {
        this.gameObject.SetActive(false);
    }
}
