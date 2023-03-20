using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slim : Monster
{
    // �Ƿ��ƶ�
    public bool moveFlag = false;

    private void Update()
    {
        if (moveFlag)
        {
            Move();
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

    // ��ʼ�ƶ�
    public void StartMove()
    {
        moveFlag = true;
    }

    // �����ƶ�
    public void EndMove()
    {
        moveFlag = false;
    }
}
