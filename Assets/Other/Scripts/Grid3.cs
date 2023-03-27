using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;
using Soultia.Voxel;
using System;



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
