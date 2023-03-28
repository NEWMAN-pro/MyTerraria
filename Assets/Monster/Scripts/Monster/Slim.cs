using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slim : Monster
{
    // 是否移动
    public bool moveFlag = false;

    private void Update()
    {
        Vector3 velocity = this.transform.GetComponent<Rigidbody>().velocity;
        if (velocity.magnitude != 0)
        {
            base.animator.SetBool("Move", true);
        }
        else
        {
            base.animator.SetBool("Move", false);
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
}
