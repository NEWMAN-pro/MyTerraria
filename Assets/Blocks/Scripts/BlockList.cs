using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储所有的Block对象的信息
/// </summary>
public class BlockList : MonoBehaviour
{
    public static Dictionary<byte, Block> blocks = new Dictionary<byte, Block>();

    // 将类的构造函数设为私有，防止外部创建新的实例
    private BlockList() { }

    void Awake()
    {
        if(blocks.Count != 0)
        {
            return;
        }
        // 土块
        Block dirt = new(1, "Dirt", 2, 2, 31);
        dirt.ID = Item.Count++;
        blocks.Add(dirt.id, dirt);

        // 草快
        Block grass = new(2, "Grass", 2, 3, 31, 0, 31, 2, 31);
        grass.ID = Item.Count++;
        blocks.Add(grass.id, grass);

        // 石块
        Block stone = new(3, "Stone", 4, 1, 31);
        stone.ID = Item.Count++;
        blocks.Add(stone.id, stone);

        // 沙子
        Block sand = new(4, "Sand", 1, 2, 30);
        sand.ID = Item.Count++;
        blocks.Add(sand.id, sand);

        // 基岩
        Block bedrock = new(5, "Bedrock", -1, 3, 18);
        bedrock.ID = Item.Count++;
        blocks.Add(bedrock.id, bedrock);

        // 树干
        Block trunk = new(6, "Trunk", 3, 4, 24, 4, 25, 4, 25);
        trunk.ID = Item.Count++;
        blocks.Add(trunk.id, trunk);

        // 树叶
        Block leaf = new(7, "Leaf", 0.5f, 4, 29);
        leaf.ID = Item.Count++;
        leaf.lucency = true;
        blocks.Add(leaf.id, leaf);

        // 木板
        Block plank = new(9, "Plank", 3, 7, 29);
        plank.ID = Item.Count++;
        plank.materials.Add(new(trunk.ID, 4, 1));
        blocks.Add(plank.id, plank);

        // 宝箱
        Block box = new(8, "Box", 2, 11, 30, 10, 30, 9, 30, 9, 30);
        box.ID = Item.Count++;
        box.materials.Add(new(plank.ID, 1, 4));
        blocks.Add(box.id, box);

        foreach(var pair in blocks)
        {
            pair.Value.type = Type.Block;
            pair.Value.count = 0;
        }
    }

    // 判断是否存在该方块
    public static Block GetBlock(byte id)
    {
        return blocks.ContainsKey(id) ? blocks[id] : null;
    }

    private void OnDestroy()
    {
        //blocks.Clear();
    }
}