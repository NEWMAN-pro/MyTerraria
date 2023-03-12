using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationState : MonoBehaviour
{
    // 动画
    public Animator animator;

    // 单次攻击
    public void SetAttackOne()
    {
        animator.SetTrigger("Attack");
        // 设置遮罩
        animator.SetLayerWeight(2, 1);
        animator.SetLayerWeight(1, 0);
    }

    // 持续攻击
    public void SetAttack(bool attack)
    {
        animator.SetBool("Attacking", attack);
        animator.SetLayerWeight(2, 1);
        animator.SetLayerWeight(1, 0);
    }

    // 单词挥动
    public void SetExcavateOne()
    {
        animator.SetTrigger("Excavate");
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(2, 0);
    }

    // 持续挥动
    public void SetExcavate(bool excavate)
    {
        animator.SetBool("Excavateing", excavate);
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(2, 0);
    }

    // 移动
    public void SetWalk(bool walk)
    {
        animator.SetBool("Walk", walk);
    }

    // 跳跃
    public void SetJump(bool jump)
    {
        animator.SetBool("Jump", jump);
    }
}
