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
    // 寻路范围
    public int range;
    // 动画
    public Animator animator;

    // 移动
    public virtual void Move() { }

    // 攻击
    public virtual void Attack() { }

    // 受伤
    public virtual void Hurt() { }

    // 死亡
    public virtual void Die() { }
}
