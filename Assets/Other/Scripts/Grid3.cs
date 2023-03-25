using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;
using Soultia.Voxel;
using System;

// 网格节点类
public class Node : IComparable<Node>
{
    // 地图上的位置
    public Vector3 posi;
    // 区块坐标
    public Vector3i chunkPosi;
    // 区块中方块坐标
    public Vector3i blockPosi;
    // 父节点
    public Node parent;
    // 代价值
    public int GCost;
    // 启发式值
    public int HCost;
    // 总代价
    public int FCost { get { return GCost + HCost; } }

    public Node(Vector3 posi, Vector3i chunkPosi, Vector3i blockPosi)
    {
        this.posi = posi;
        this.chunkPosi = chunkPosi;
        this.blockPosi = blockPosi;
    }

    public Node()
    {
    }

    // 判断是否相等
    public bool Equal(Node other)
    {
        return chunkPosi == other.chunkPosi && blockPosi == other.blockPosi;
    }

    // 重载比较函数
    public int CompareTo(Node other)
    {
        // 先比较FCost的大小在比较HCost的大小，从小到大排序
        if(FCost == other.FCost)
        {
            if(HCost > other.HCost)
            {
                return 1;
            }
            else if(HCost < other.HCost)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if(FCost > other.FCost)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}

public class Grid3 : MonoBehaviour
{
    // 网格起始区块
    public Vector3i startPosi;
    // 节点信息列表
    public Dictionary<Vector3i, Node> grids = new();

    private void Update()
    {
        
    }

    // 创建网格
    public void CreateGrid(Vector3i posi)
    {
        this.startPosi = posi;

    }

    // 获取指定位置的节点
    public Node GetNode(Vector3i posi)
    {
        //// 获得目标点所在的区块坐标
        //Vector3i targetPosi = new(startPosi.x + (posi.x / 16), startPosi.y + (posi.y / 16), startPosi.z + (posi.z / 16));
        //// 获取目标点在其所在区块中的坐标
        //Vector3i chunkPosi = new(posi.x % 16, posi.y % 16, posi.z % 16);
        return grids.ContainsKey(posi) ? grids[posi] : null;
    }

    // 将世界坐标转化为节点
    public Node NodeFromWorldPoint(Vector3 posi)
    {
        Vector3 gridPosi = posi - startPosi;
        return GetNode(new((int)Mathf.Floor(gridPosi.x), (int)Mathf.Floor(gridPosi.y), (int)Mathf.Floor(gridPosi.z)));
    }
}
