using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : MonoBehaviour
{
    // ��ҳ�����
    public Vector3 home = new(0, 25, 0);
    // �������
    public string playName = "";
    // ���Ѫ��
    public int HP = 100;
    // ���Ѫ��
    public int maxHP = 100;
    // �������
    public int MP = 100;
    // �������
    public int maxMP = 100;
    // ����
    public int defenes = 0;
    // �˺�
    public int damage = 10;

    // ���״̬��
    public State state;

    private void Awake()
    {
        if (!StartUI.flag)
        {
            // ����Ǽ�����Ϸ
            GameData data = AccessGameAll.data;
            this.playName = data.playerName;
            this.maxHP = data.maxHP;
            this.maxMP = data.maxMP;
            this.HP = this.maxHP;
            this.MP = this.maxMP;
        }
        else
        {
            this.playName = StartUI.playerName;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameObject.Find("UI").transform.GetChild(6).GetComponent<State>();
        state.CreateUI(HP, maxHP, true);
        state.CreateUI(MP, maxMP, false);
    }

    // Ѫ���仯
    public void SetHP(int hp)
    {
        HP = (HP + hp) <= maxHP ? (HP + hp) : maxHP;
        if(HP <= 0)
        {
            Debug.Log("�������");
            return;
        }
        state.CreateUI(HP, maxHP, true);
    }

    // �����仯
    public void SetMP(int mp)
    {
        MP = (MP + mp) <= maxMP ? (MP + mp) : maxMP;
        if (MP < 0) MP = 0;
        state.CreateUI(MP, maxMP, false);
    }

    // ���ó�����
    public void SetHome(Vector3 posi)
    {
        this.home = posi;
    }
}
