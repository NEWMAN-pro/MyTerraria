using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slim : Monster
{
    // 是否移动
    public bool moveFlag = false;

    // 目标
    public GameObject player;

    private void Update()
    {
        if (moveFlag)
        {
            base.animator.SetBool("Move", true);
        }
        else
        {
            base.animator.SetBool("Move", false);
        }
    }

    public override void Move()
    {
        //this.transform.position += Vector3.Scale(this.transform.forward, new Vector3(0, 0, 0.01f));
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
}
