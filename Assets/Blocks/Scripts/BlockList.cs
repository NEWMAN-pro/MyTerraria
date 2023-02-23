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
        Block dirt = new Block(1, "Dirt", 3, 2, 31);
        blocks.Add(dirt.id, dirt);

        // 草快
        Block grass = new Block(2, "Grass", 3, 3, 31, 0, 31, 2, 31);
        blocks.Add(grass.id, grass);

        // 石块
        Block stone = new Block(3, "Stone", 5, 1, 31);
        blocks.Add(stone.id, stone);

        // 沙子
        Block sand = new Block(4, "Sand", 2, 2, 30);
        blocks.Add(sand.id, sand);
    }

    // 判断是否存在该方块
    public static Block GetBlock(byte id)
    {
        return blocks.ContainsKey(id) ? blocks[id] : null;
    }
}