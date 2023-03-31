using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slim : Monster
{
    // 是否移动
    public bool moveFlag = false;

    private void Start()
    {
        base.aStar = this.transform.GetComponent<AStar>();
        base.target = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        AnimatorController();
        if(Vector3.Distance(base.target.position, this.transform.position) <= base.range)
        {
            // 进入寻路距离，开始寻路
            base.aStar.target = base.target;
            base.aStar.flag = true;
        }
        else
        {
            base.aStar.flag = false;
        }
    }

    public override void Attack()
    {
        Debug.Log("Slim Attack");
    }

    public override void Hurt()
    {
        base.Hurt();
    }

    public override void Die()
    {
        base.Die();
    }

    public override void AnimatorController()
    {
        bool AttackFlag = base.aStar.AttackFlag;
        if (AttackFlag)
        {
            base.animator.SetBool("Attack", true);
            base.animator.SetBool("Move", false);
        }
        else
        {
            base.animator.SetBool("Attack", false);
            bool MoveFlag = base.aStar.flag;
            if (MoveFlag)
            {
                base.animator.SetBool("Move", true);
            }
            else
            {
                base.animator.SetBool("Move", false);
            }
        }
    }
}
