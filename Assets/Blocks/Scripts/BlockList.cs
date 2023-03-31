using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储所有的Block对象的信息
/// </summary>
public class BlockList : MonoBehaviour
{
    public static Dictionary<byte, Block> blocks = new Dictionary<byte, Block>();

    void Awake()
    {
        // 土块
        Block dirt = new Block(1, "Dirt", 2, 2, 31);
        blocks.Add(dirt.id, dirt);

        // 草快
        Block grass = new Block(2, "Grass", 2, 3, 31, 0, 31, 2, 31);
        blocks.Add(grass.id, grass);

        // 石块
        Block stone = new Block(3, "Stone", 4, 1, 31);
        blocks.Add(stone.id, stone);

        // 沙子
        Block sand = new Block(4, "Sand", 1, 2, 30);
        blocks.Add(sand.id, sand);

        // 基岩
        Block bedrock = new Block(5, "Bedrock", -1, 3, 18);
        blocks.Add(bedrock.id, bedrock);

        // 树干
        Block trunk = new Block(6, "Trunk", 3, 4, 24, 4, 25, 4, 25);
        blocks.Add(trunk.id, trunk);

        // 树叶
        Block leaf = new Block(7, "Leaf", 0.5f, 4, 29);
        leaf.lucency = true;
        blocks.Add(leaf.id, leaf);

        // 宝箱
        Block box = new Block(8, "Box", 2, 11, 30, 10, 30, 11, 27, 9, 30);
        blocks.Add(box.id, box);
    }

    // 判断是否存在该方块
    public static Block GetBlock(byte id)
    {
        return blocks.ContainsKey(id) ? blocks[id] : null;
    }

    private void OnDestroy()
    {
        blocks.Clear();
    }
}