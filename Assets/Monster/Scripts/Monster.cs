using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    // Ѫ��
    public int HP;
    // ���Ѫ��
    public int maxHP;
    // ����
    public int defenes;
    // �˺�
    public int damage;
    // �ƶ��ٶ�
    public float speed;
    // ��Ծ�߶�
    public float jump;
    // Ѱ·Ŀ��
    public Transform traget;

    // �ƶ�
    public virtual void Move() { }

    // ����
    public virtual void Attack() { }

    // ����
    public virtual void Hurt() { }

    // ����
    public virtual void Die() { }
}
