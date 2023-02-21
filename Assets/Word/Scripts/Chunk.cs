using System;
using Soultia.Util;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Soultia.Voxel
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        // 区块的宽
        public static int width = 16;
        // 区块的高
        public static int height = 16;

        // 区块方块表
        public byte[,,] blocks;
        // 区块位置
        public Vector3i position;

        // 方块销毁时间
        private float destroyTime;

        private Mesh mesh;

        //面需要的点
        private List<Vector3> vertices = new List<Vector3>();
        //生成三边面时用到的vertices的index
        private List<int> triangles = new List<int>();

        //所有的uv信息
        private List<Vector2> uv = new List<Vector2>();
        //uv贴图每行每列的宽度(0~1)，这里我的贴图是32×32的，所以是1/32
        public static float textureOffset = 1 / 32f;
        //让UV稍微缩小一点，避免出现它旁边的贴图
        public static float shrinkSize = 0.001f;


        //当前Chunk是否正在生成中
        private bool isWorking = false;
        private bool isFinished = false;

        void Start()
        {
            position = new Vector3i(this.transform.position);
            if (Map.instance.ChunkExists(position))
            {
                Debug.Log("此方块已存在" + position);
                Destroy(this);
            }
            else
            {
                Map.instance.chunks.Add(position, this.gameObject);
                this.name = "(" + position.x + "," + position.y + "," + position.z + ")";
                //StartFunction();
            }
        }

        private void Update()
        {
            if (isWorking == false && isFinished == false)
            {
                isFinished = true;
                StartFunction();
            }
        }

        void StartFunction()
        {
            isWorking = true;
            mesh = new Mesh();
            mesh.name = "Chunk";

            StartCoroutine(CreateMap());
        }

        IEnumerator CreateMap()
        {
            blocks = new byte[width, height, width];
            for (int x = 0; x < Chunk.width; x++)
            {
                for (int y = 0; y < Chunk.height; y++)
                {
                    for (int z = 0; z < Chunk.width; z++)
                    {
                        byte blockid = Terrain.GetTerrainBlock(new Vector3i(x, y, z) + position);
                        if (blockid == 1 && Terrain.GetTerrainBlock(new Vector3i(x, y + 1, z) + position) == 0)
                        {
                            blocks[x, y, z] = 2;
                        }
                        else
                        {
                            blocks[x, y, z] = Terrain.GetTerrainBlock(new Vector3i(x, y, z) + position);
                        }
                    }
                }
            }

            yield return null;
            StartCoroutine(CreateMesh());
        }

        IEnumerator CreateMesh()
        {
            vertices.Clear();
            triangles.Clear();
            uv.Clear();

            //把所有面的点和面的索引添加进去
            for (int x = 0; x < Chunk.width; x++)
            {
                for (int y = 0; y < Chunk.height; y++)
                {
                    for (int z = 0; z < Chunk.width; z++)
                    {
                        //获取当前坐标的Block对象
                        Block block = BlockList.GetBlock(this.blocks[x, y, z]);
                        if (block == null) continue;

                        if (IsBlockTransparent(x + 1, y, z))
                        {
                            AddFrontFace(x, y, z, block);
                        }
                        if (IsBlockTransparent(x - 1, y, z))
                        {
                            AddBackFace(x, y, z, block);
                        }
                        if (IsBlockTransparent(x, y, z + 1))
                        {
                            AddRightFace(x, y, z, block);
                        }
                        if (IsBlockTransparent(x, y, z - 1))
                        {
                            AddLeftFace(x, y, z, block);
                        }
                        if (IsBlockTransparent(x, y + 1, z))
                        {
                            AddTopFace(x, y, z, block);
                        }
                        if (IsBlockTransparent(x, y - 1, z))
                        {
                            AddBottomFace(x, y, z, block);
                        }
                    }
                }
            }


            //为点和index赋值
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            if(mesh.vertices.Length == uv.Count)
            {
                mesh.uv = uv.ToArray();
            }

            //重新计算顶点和法线
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            //将生成好的面赋值给组件
            this.GetComponent<MeshFilter>().mesh = mesh;
            this.GetComponent<MeshCollider>().sharedMesh = mesh;

            yield return null;
            isWorking = false;
        }

        //此坐标方块是否透明，Chunk中的局部坐标
        public bool IsBlockTransparent(int x, int y, int z)
        {
            if (x >= width || y >= height || z >= width || x < 0 || y < 0 || z < 0)
            {
                return true;
            }
            else
            {
                //如果当前方块的id是0，那的确是透明的
                return this.blocks[x, y, z] == 0;
            }
        }

        //前面
        void AddFrontFace(int x, int y, int z, Block block)
        {
            //第一个三角面
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);

            //第二个三角面
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);


            //添加4个点
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));

            //添加UV坐标点，跟上面4个点循环的顺序一致
            uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //背面
        void AddBackFace(int x, int y, int z, Block block)
        {
            //第一个三角面
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);

            //第二个三角面
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);


            //添加4个点
            vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));

            //添加UV坐标点，跟上面4个点循环的顺序一致
            uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //右面
        void AddRightFace(int x, int y, int z, Block block)
        {
            //第一个三角面
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);

            //第二个三角面
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);


            //添加4个点
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));

            //添加UV坐标点，跟上面4个点循环的顺序一致
            uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //左面
        void AddLeftFace(int x, int y, int z, Block block)
        {
            //第一个三角面
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);

            //第二个三角面
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);


            //添加4个点
            vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));

            //添加UV坐标点，跟上面4个点循环的顺序一致
            uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //上面
        void AddTopFace(int x, int y, int z, Block block)
        {
            //第一个三角面
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);

            //第二个三角面
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);


            //添加4个点
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));

            //添加UV坐标点，跟上面4个点循环的顺序一致
            uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //下面
        void AddBottomFace(int x, int y, int z, Block block)
        {
            //第一个三角面
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);

            //第二个三角面
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);


            //添加4个点
            vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));

            //添加UV坐标点，跟上面4个点循环的顺序一致
            uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        // 世界坐标转区块坐标
        public Vector3i WorldTransferChunk(Vector3 position)
        {
            return new Vector3i(position) - this.position - new Vector3i(-1, 0, 0);
        }

        // 区块坐标转世界坐标
        public Vector3 ChunkTransferWorld(Vector3i position)
        {
            return position.ToVector3() + new Vector3(-0.5f, 0.5f, 0.5f) + this.position;
        }

        // 生成方块
        public byte CreateBlock(Vector3 position, Vector3 playPosition)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);
            Vector3 wordPosition = ChunkTransferWorld(chunkPosition);

            // 如果与角色重合，则不生成
            if(Math.Abs(wordPosition.x - playPosition.x) < 1 && Math.Abs(wordPosition.y - playPosition.y) < 1.5f && Math.Abs(wordPosition.z - playPosition.z) < 1)
            {
                Debug.Log("C与角色重合");
                return 1;
            }

            // 如果超过区块界限，则不生成
            if(chunkPosition.x >= Chunk.width || chunkPosition.y >= Chunk.height || chunkPosition.z >= Chunk.width || chunkPosition.x < 0 || chunkPosition.y < 0 || chunkPosition.z < 0)
            {
                Debug.Log("C越界");
                return 2;
            }

            // 该位置已有方块
            if (blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] != 0)
            {
                Debug.Log("C该位置已有方块");
                return 3;
            }
            Debug.Log("生成");
            isWorking = true;
            mesh = new Mesh();
            mesh.name = "Chunk";
            blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] = 1;
            StartCoroutine(CreateMesh());
            return 0;
        }

        // 销毁方块
        public byte DestroyBlock(Vector3 position)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);

            // 如果超过区块界限
            if (chunkPosition.x >= Chunk.width || chunkPosition.y >= Chunk.height || chunkPosition.z >= Chunk.width || chunkPosition.x < 0 || chunkPosition.y < 0 || chunkPosition.z < 0)
            {
                Debug.Log("D越界");
                return 1;
            }

            // 该位置没有方块
            if (blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] == 0)
            {
                Debug.Log("D该位置没有方块");
                return 2;
            }
            Debug.Log(destroyTime);
            // 判断是否销毁够时间
            if(destroyTime >= 0)
            {
                // 不够则更新时间
                destroyTime -= Time.deltaTime;
                return 3;
            }
            Debug.Log("销毁");
            isWorking = true;
            mesh = new Mesh();
            mesh.name = "Chunk";
            blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] = 0;
            StartCoroutine(CreateMesh());
            return 0;

        }

        // 刷新时间
        public void setDestroyTime(Vector3 position)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);

            Block block = BlockList.GetBlock(this.blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z]);
            if (block == null) return;
            destroyTime = block.destroyTime;
        }
    }
}