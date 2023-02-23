using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �洢���е�Block�������Ϣ
/// </summary>
public class BlockList : MonoBehaviour
{
    public static Dictionary<byte, Block> blocks = new Dictionary<byte, Block>();

    void Awake()
    {
        // ����
        Block dirt = new Block(1, "Dirt", 3, 2, 31);
        blocks.Add(dirt.id, dirt);

        // �ݿ�
        Block grass = new Block(2, "Grass", 3, 3, 31, 0, 31, 2, 31);
        blocks.Add(grass.id, grass);

        // ʯ��
        Block stone = new Block(3, "Stone", 5, 1, 31);
        blocks.Add(stone.id, stone);

        // ɳ��
        Block sand = new Block(4, "Sand", 2, 2, 30);
        blocks.Add(sand.id, sand);
    }

    // �ж��Ƿ���ڸ÷���
    public static Block GetBlock(byte id)
    {
        return blocks.ContainsKey(id) ? blocks[id] : null;
    }
}