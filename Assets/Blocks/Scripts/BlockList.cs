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
        Block dirt = new Block(1, "Dirt", 2, 2, 31);
        blocks.Add(dirt.id, dirt);

        // �ݿ�
        Block grass = new Block(2, "Grass", 2, 3, 31, 0, 31, 2, 31);
        blocks.Add(grass.id, grass);

        // ʯ��
        Block stone = new Block(3, "Stone", 4, 1, 31);
        blocks.Add(stone.id, stone);

        // ɳ��
        Block sand = new Block(4, "Sand", 1, 2, 30);
        blocks.Add(sand.id, sand);

        // ����
        Block bedrock = new Block(5, "Bedrock", -1, 3, 18);
        blocks.Add(bedrock.id, bedrock);

        // ����
        Block trunk = new Block(6, "Trunk", 3, 4, 24, 4, 25, 4, 25);
        blocks.Add(trunk.id, trunk);

        // ��Ҷ
        Block leaf = new Block(7, "Leaf", 0.5f, 4, 29);
        leaf.lucency = true;
        blocks.Add(leaf.id, leaf);

        // ����
        Block box = new Block(8, "Box", 2, 11, 30, 10, 30, 11, 27, 9, 30);
        blocks.Add(box.id, box);
    }

    // �ж��Ƿ���ڸ÷���
    public static Block GetBlock(byte id)
    {
        return blocks.ContainsKey(id) ? blocks[id] : null;
    }
}