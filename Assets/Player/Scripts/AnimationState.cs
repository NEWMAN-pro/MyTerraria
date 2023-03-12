using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationState : MonoBehaviour
{
    // ����
    public Animator animator;

    // ���ι���
    public void SetAttackOne()
    {
        animator.SetTrigger("Attack");
        // ��������
        animator.SetLayerWeight(2, 1);
        animator.SetLayerWeight(1, 0);
    }

    // ��������
    public void SetAttack(bool attack)
    {
        animator.SetBool("Attacking", attack);
        animator.SetLayerWeight(2, 1);
        animator.SetLayerWeight(1, 0);
    }

    // ���ʻӶ�
    public void SetExcavateOne()
    {
        animator.SetTrigger("Excavate");
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(2, 0);
    }

    // �����Ӷ�
    public void SetExcavate(bool excavate)
    {
        animator.SetBool("Excavateing", excavate);
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(2, 0);
    }

    // �ƶ�
    public void SetWalk(bool walk)
    {
        animator.SetBool("Walk", walk);
    }

    // ��Ծ
    public void SetJump(bool jump)
    {
        animator.SetBool("Jump", jump);
    }
}
