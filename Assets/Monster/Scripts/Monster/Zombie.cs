﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Monster
{
    private void Awake()
    {
        base.HP = 100;
        base.maxHP = 100;
        base.defenes = 2;
        base.damage = 10;
        base.speed = 5f;
        base.range = 8;
        base.attackRange = 1.8f;
    }

    private void OnEnable()
    {
        base.HP = 100;
        base.maxHP = 100;
        base.defenes = 2;
        base.damage = 10;
        base.speed = 5f;
        base.range = 8;
        base.attackRange = 1.8f;
        this.transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void Update()
    {
        base.Update();
        AnimatorController();
        if (Vector3.Distance(base.target.position, this.transform.position) <= base.range)
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
        if (Vector3.Distance(this.transform.position, target.position) <= base.attackRange)
        {
            target.GetComponent<PlayState>().SetHP(-base.damage);
        }
    }

    public override void SetHP(int hp)
    {
        base.SetHP(hp);
    }

    public override void Die()
    {
        base.Die();
        Debug.Log("Zombie Die");
        base.animator.SetTrigger("Die");
    }

    public override void AnimatorController()
    {
        bool AttackFlag = base.aStar.AttackFlag;
        if (AttackFlag)
        {
            base.animator.SetBool("Attack", true);
            base.animator.SetBool("Move", false);
            base.animator.SetLayerWeight(1, 1);
        }
        else
        {
            base.animator.SetBool("Attack", false);
            base.animator.SetLayerWeight(1, 0);
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
