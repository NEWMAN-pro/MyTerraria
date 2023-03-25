using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soultia.Util;
using Soultia.Voxel;

public class AStar : MonoBehaviour
{
    // 寻路起点和终点
    public Transform seeker, target;
    // 开放节点优先队列
    PriorityQueue<Node> openSet = new();
    // 已考虑过节点集合
    List<Node> clostSet = new();
    // 路径
    public List<Vector3> path = new();
    // 是否开启寻路
    public bool flag = false;
    // 是否在寻路
    public bool isFind = false;
    // 是否寻路成功
    public bool FindFlag = false;

    readonly int[] X = { 0, 0, 1, -1, 1, -1, 1, -1 };
    readonly int[] Z = { 1, -1, 0, 0, -1, 1, 1, -1 };

    private void Awake()
    {
        seeker = this.transform;
        //target = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (flag)
        {
            if (!isFind)
            {
                // 开始寻路，每两秒更新一次路径
                //InvokeRepeating(nameof(FindPath), 0, 2);
                FindPath();
                isFind = true;
            }
            if (!FindFlag)
            {
                // 寻路失败
                Debug.Log("无路可走");
            }
            else
            {
                WalkForPath();
                flag = false;
            }
        }
        else
        {
            if (isFind)
            {
                // 停止寻路
                CancelInvoke();
                isFind = false;
            }
        }
    }

    // 跟着路径行走
    public void WalkForPath()
    {
        Vector3 nowPosi = path[0];
        for(int i = 1; i < path.Count; i++)
        {
            //Debug.Log(path[i]);
            //Debug.DrawLine(nowPosi, path[i], Color.blue);
            nowPosi = path[i];
        }
    }

    // 寻路
    public void FindPath()
    {
        openSet.Clear();
        clostSet.Clear();

        Vector3 startPosi = seeker.position;
        Vector3 targetPosi = target.position;
        startPosi = NormPosi(startPosi);
        targetPosi = NormPosi(targetPosi);
        targetPosi.y -= 1;

        // 获取起始节点信息
        Node startNode = new(startPosi, WorldToChunkPosi(startPosi), WorldToBlockPosi(startPosi));
        Node targetNode = new(targetPosi, WorldToChunkPosi(targetPosi), WorldToBlockPosi(targetPosi));
        Debug.Log("StartPosi: " + startNode.chunkPosi + " " + startNode.blockPosi);
        Debug.Log("TargetPosi: " + targetNode.chunkPosi + " " + targetNode.blockPosi);

        // 起点入队列
        openSet.Enqueue(startNode);


        int count = 0;
        // 寻路
        while(openSet.Count > 0)
        {
            if (count++ > 100)
            {
                Debug.Log("0000");
                return;
            }
            // 获取值最小的节点
            Node node = openSet.Dequeue();
            Debug.Log("now: " + node.chunkPosi + " " + node.blockPosi + " " + node.FCost);

            clostSet.Add(node);

            //if(node == targetNode)
            if(node.Equal(targetNode))
            {
                // 如果到达终点
                targetNode.parent = node.parent;
                RetracePath(startNode, targetNode);
                FindFlag = true;
                Debug.Log("1");
                return;
            }

            // 遍历周围八个节点
            for(int i = 0; i < 8; i++)
            {
                // 新的节点
                Node newNode = new(node.posi, node.chunkPosi, node.blockPosi);
                newNode.blockPosi.x = node.blockPosi.x + X[i];
                newNode.blockPosi.z = node.blockPosi.z + Z[i];
                newNode.posi.x = node.posi.x + X[i];
                newNode.posi.z = node.posi.z + Z[i];
                GetNewBlockPosi(ref newNode);

                if (Limit(newNode, targetNode))
                {
                    // 超过寻路范围
                    Debug.Log("11");
                    continue;
                }

                if (ClostSetFind(newNode))
                {
                    // 如果该该节点已经被删除，则跳过
                    Debug.Log("22");
                    continue;
                }
                int state = GetBlockState(newNode);
                if(state == 0)
                {
                    // 如果是空方块，判断其下面的方块
                    Node belowNode = new(newNode.posi, newNode.chunkPosi, newNode.blockPosi);
                    belowNode.posi.y -= 1;
                    belowNode.blockPosi.y -= 1;
                    GetNewBlockPosi(ref belowNode);
                    int belowState = GetBlockState(belowNode);
                    if(belowState == 0)
                    {
                        // 如果下面方块为空，不可以行走，继续判断下面方块，否则可以行走
                        Node nextBelowNode = new(belowNode.posi, belowNode.chunkPosi, belowNode.blockPosi);
                        nextBelowNode.posi.y -= 1;
                        nextBelowNode.blockPosi.y -= 1;
                        GetNewBlockPosi(ref nextBelowNode);
                        if(GetBlockState(nextBelowNode) == 0)
                        {
                            // 如何还是空，则不能行走，跳过该点
                            Debug.Log("33");
                            continue;
                        }
                        else
                        {
                            // 如果不为空，则可行走，更新到达的方块
                            newNode = belowNode;
                        }
                    }
                }
                else
                {
                    // 如果不是空方块，判断其上面的方块
                    Node aboveNode = new(newNode.posi, newNode.chunkPosi, newNode.blockPosi);
                    aboveNode.posi.y += 1;
                    aboveNode.blockPosi.y += 1;
                    GetNewBlockPosi(ref aboveNode);
                    int aboveState = GetBlockState(aboveNode);
                    if(aboveState != 0)
                    {
                        // 如果上面方块不为空，则不能走，跳过该点
                        Debug.Log("44");
                        continue;
                    }
                    else
                    {
                        // 如果上面方块为空，则判断其上面方块，看空间是否足够
                        Node nextAboveNode = new(aboveNode.posi, aboveNode.chunkPosi, aboveNode.blockPosi);
                        nextAboveNode.posi.y += 1;
                        nextAboveNode.blockPosi.y += 1;
                        GetNewBlockPosi(ref nextAboveNode);
                        if(GetBlockState(nextAboveNode) != 0)
                        {
                            // 如果不为空，空间不足，不可行走
                            Debug.Log("55");
                            continue;
                        }
                        else
                        {
                            // 空间足够，可以行走
                            newNode = aboveNode;
                        }
                    }
                }

                if (Limit(newNode, targetNode))
                {
                    // 超过寻路范围
                    Debug.Log("11");
                    continue;
                }

                if (ClostSetFind(newNode))
                {
                    // 如果该该节点已经被删除，则跳过
                    Debug.Log("22");
                    continue;
                }

                int dis = node.GCost + GetDistance(node, newNode);
                if (!openSet.Contains(newNode))
                {
                    // 如果该节点不在队列中，更新信息并入队
                    newNode.GCost = dis;
                    newNode.HCost = GetDistance(newNode, targetNode);
                    newNode.parent = node;
                    //Debug.Log("NowNodeParent: " + newNode.parent.chunkPosi + " " + newNode.parent.blockPosi);
                    openSet.Enqueue(newNode);
                }
            }
        }
        // 如果没找到终点
        FindFlag = false;
        Debug.Log("2");
        return ;
    }

    // 判断node是否在删除列表中
    public bool ClostSetFind(Node targetNode)
    {
        // 判断是否存在位置相同的节点
        return clostSet.Exists(node => node.chunkPosi == targetNode.chunkPosi && node.blockPosi == targetNode.blockPosi);
    }

    // 判断是否越界
    public bool Limit(Node node1, Node node2)
    {
        Vector3i dis = node1.chunkPosi - node2.chunkPosi;
        if(Mathf.Abs(dis.x) > 16 || Mathf.Abs(dis.y) > 16 || Mathf.Abs(dis.z) > 16)
        {
            return true;
        }
        return false;
    }

    // 获取当前坐标方块的状态
    public int GetBlockState(Node node)
    {
        string name = "(" + node.chunkPosi.x + "," + node.chunkPosi.y + "," + node.chunkPosi.z + ")";
        return GameObject.Find(name).GetComponent<Chunk>().blocks[node.blockPosi.x, node.blockPosi.y, node.blockPosi.z];
    }

    // 获取路径
    void RetracePath(Node startNode, Node endNode)
    {
        path.Clear();
        Node node = endNode;
        // 反推路径
        while (node != null && !node.Equal(startNode))
        {
            path.Add(node.posi);
            node = node.parent;
        }
        path.Add(startNode.posi);
        path.Reverse();
    }

    // 计算新的节点的坐标信息
    public void GetNewBlockPosi(ref Node node)
    {
        if(node.blockPosi.x < 0)
        {
            node.chunkPosi.x -= 16;
        }
        else if(node.blockPosi.x > 15)
        {
            node.chunkPosi.x += 16;
        }
        if(node.blockPosi.y < 0)
        {
            node.chunkPosi.y -= 16;
        }
        else if(node.blockPosi.y > 15)
        {
            node.chunkPosi.y += 16;
        }
        if(node.blockPosi.z < 0)
        {
            node.chunkPosi.z -= 16;
        }
        else if(node.blockPosi.z > 15)
        {
            node.chunkPosi.z += 16;
        }
        node.blockPosi.x = (node.blockPosi.x + 16) % 16;
        node.blockPosi.y = (node.blockPosi.y + 16) % 16;
        node.blockPosi.z = (node.blockPosi.z + 16) % 16;
    }

    // 获得所在区块坐标
    public Vector3i WorldToChunkPosi(Vector3 posi)
    {
        posi.x += 1;
        Vector3i chunkPosi = new((int)(posi.x / 16) * 16, (int)(posi.y / 16) * 16, (int)(posi.z / 16) * 16);
        chunkPosi.x += posi.x < 0 ? -16 : 0;
        chunkPosi.y += posi.y < 0 ? -16 : 0;
        chunkPosi.z += posi.z < 0 ? -16 : 0;
        return chunkPosi;
    }

    // 获取其在区块中的坐标
    public Vector3i WorldToBlockPosi(Vector3 posi)
    {
        Vector3i chunkPosi = WorldToChunkPosi(posi);
        return new Vector3i(posi) - chunkPosi - new Vector3i(-1, 0, 0);
    }

    // 计算节点之间的距离
    public int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.blockPosi.x - nodeB.blockPosi.x + nodeA.chunkPosi.x - nodeB.chunkPosi.x);
        int distY = Mathf.Abs(nodeA.blockPosi.y - nodeB.blockPosi.y + nodeA.chunkPosi.y - nodeB.chunkPosi.y);
        int distZ = Mathf.Abs(nodeA.blockPosi.z - nodeB.blockPosi.z + nodeA.chunkPosi.z - nodeB.chunkPosi.z);

        if(distX < distZ)
        {
            return 14 * distX + 10 * (distZ - distX + distY);
        }
        return 14 * distZ + 10 * (distX - distZ + distY);
    }

    // 规范化坐标
    public Vector3 NormPosi(Vector3 posi)
    {
        return new(Mathf.Ceil(posi.x) + 0.5f, Mathf.Ceil(posi.y) + 0.5f, Mathf.Ceil(posi.z) + 0.5f);
    }
}
