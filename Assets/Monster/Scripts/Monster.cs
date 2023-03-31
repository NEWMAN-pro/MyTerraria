using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    // 血量
    public int HP;
    // 最大血量
    public int maxHP;
    // 防御
    public int defenes;
    // 伤害
    public int damage;
    // 移动速度
    public float speed;
    // 跳跃高度
    public float jump;
    // 是否死亡
    public bool dieFlag = false;
    // 攻击方位
    public float attackRange;

    // 寻路范围
    public float range;
    // 寻路目标
    public Transform target;
    // 动画
    public Animator animator;
    // 寻路算法
    public AStar aStar;

    public void Start()
    {
        aStar = this.transform.GetComponent<AStar>();
        aStar.speed = speed;
        aStar.stopRange = attackRange;
        target = GameObject.Find("Player").transform;
    }

    // 移动
    public virtual void Move() { }

    // 攻击
    public virtual void Attack() { }

    // 受伤
    public virtual void SetHP(int hp)
    {
        HP = (HP + hp) <= maxHP ? (HP + hp) : maxHP;
        if (HP <= 0)
        {
            Die();
            return;
        }
    }

    // 死亡
    public virtual void Die()
    {
        Debug.Log("怪物死亡");
    }

    // 销毁
    public virtual void Destroy()
    {
        this.gameObject.SetActive(false);
    }

    // 动画
    public virtual void AnimatorController() { }
}
