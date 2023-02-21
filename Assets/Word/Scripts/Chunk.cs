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
        // ����Ŀ�
        public static int width = 16;
        // ����ĸ�
        public static int height = 16;

        // ���鷽���
        public byte[,,] blocks;
        // ����λ��
        public Vector3i position;

        // ��������ʱ��
        private float destroyTime;

        private Mesh mesh;

        //����Ҫ�ĵ�
        private List<Vector3> vertices = new List<Vector3>();
        //����������ʱ�õ���vertices��index
        private List<int> triangles = new List<int>();

        //���е�uv��Ϣ
        private List<Vector2> uv = new List<Vector2>();
        //uv��ͼÿ��ÿ�еĿ��(0~1)�������ҵ���ͼ��32��32�ģ�������1/32
        public static float textureOffset = 1 / 32f;
        //��UV��΢��Сһ�㣬����������Աߵ���ͼ
        public static float shrinkSize = 0.001f;


        //��ǰChunk�Ƿ�����������
        private bool isWorking = false;
        private bool isFinished = false;

        void Start()
        {
            position = new Vector3i(this.transform.position);
            if (Map.instance.ChunkExists(position))
            {
                Debug.Log("�˷����Ѵ���" + position);
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

            //��������ĵ�����������ӽ�ȥ
            for (int x = 0; x < Chunk.width; x++)
            {
                for (int y = 0; y < Chunk.height; y++)
                {
                    for (int z = 0; z < Chunk.width; z++)
                    {
                        //��ȡ��ǰ�����Block����
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


            //Ϊ���index��ֵ
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            if(mesh.vertices.Length == uv.Count)
            {
                mesh.uv = uv.ToArray();
            }

            //���¼��㶥��ͷ���
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            //�����ɺõ��渳ֵ�����
            this.GetComponent<MeshFilter>().mesh = mesh;
            this.GetComponent<MeshCollider>().sharedMesh = mesh;

            yield return null;
            isWorking = false;
        }

        //�����귽���Ƿ�͸����Chunk�еľֲ�����
        public bool IsBlockTransparent(int x, int y, int z)
        {
            if (x >= width || y >= height || z >= width || x < 0 || y < 0 || z < 0)
            {
                return true;
            }
            else
            {
                //�����ǰ�����id��0���ǵ�ȷ��͸����
                return this.blocks[x, y, z] == 0;
            }
        }

        //ǰ��
        void AddFrontFace(int x, int y, int z, Block block)
        {
            //��һ��������
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);

            //�ڶ���������
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);


            //���4����
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));

            //���UV����㣬������4����ѭ����˳��һ��
            uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureFrontX * textureOffset + textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureFrontX * textureOffset, block.textureFrontY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //����
        void AddBackFace(int x, int y, int z, Block block)
        {
            //��һ��������
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);

            //�ڶ���������
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);


            //���4����
            vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));

            //���UV����㣬������4����ѭ����˳��һ��
            uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureBackX * textureOffset + textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureBackX * textureOffset, block.textureBackY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //����
        void AddRightFace(int x, int y, int z, Block block)
        {
            //��һ��������
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);

            //�ڶ���������
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);


            //���4����
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));

            //���UV����㣬������4����ѭ����˳��һ��
            uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureRightX * textureOffset + textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureRightX * textureOffset, block.textureRightY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //����
        void AddLeftFace(int x, int y, int z, Block block)
        {
            //��һ��������
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);

            //�ڶ���������
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);


            //���4����
            vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));

            //���UV����㣬������4����ѭ����˳��һ��
            uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureLeftX * textureOffset + textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureLeftX * textureOffset, block.textureLeftY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //����
        void AddTopFace(int x, int y, int z, Block block)
        {
            //��һ��������
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);

            //�ڶ���������
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);


            //���4����
            vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
            vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));
            vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));

            //���UV����㣬������4����ѭ����˳��һ��
            uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset + textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureTopX * textureOffset, block.textureTopY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        //����
        void AddBottomFace(int x, int y, int z, Block block)
        {
            //��һ��������
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);

            //�ڶ���������
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);


            //���4����
            vertices.Add(new Vector3(-1 + x, 0 + y, 0 + z));
            vertices.Add(new Vector3(-1 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 1 + z));
            vertices.Add(new Vector3(0 + x, 0 + y, 0 + z));

            //���UV����㣬������4����ѭ����˳��һ��
            uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(block.textureBottomX * textureOffset + textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(block.textureBottomX * textureOffset, block.textureBottomY * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
        }

        // ��������ת��������
        public Vector3i WorldTransferChunk(Vector3 position)
        {
            return new Vector3i(position) - this.position - new Vector3i(-1, 0, 0);
        }

        // ��������ת��������
        public Vector3 ChunkTransferWorld(Vector3i position)
        {
            return position.ToVector3() + new Vector3(-0.5f, 0.5f, 0.5f) + this.position;
        }

        // ���ɷ���
        public byte CreateBlock(Vector3 position, Vector3 playPosition)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);
            Vector3 wordPosition = ChunkTransferWorld(chunkPosition);

            // ������ɫ�غϣ�������
            if(Math.Abs(wordPosition.x - playPosition.x) < 1 && Math.Abs(wordPosition.y - playPosition.y) < 1.5f && Math.Abs(wordPosition.z - playPosition.z) < 1)
            {
                Debug.Log("C���ɫ�غ�");
                return 1;
            }

            // �������������ޣ�������
            if(chunkPosition.x >= Chunk.width || chunkPosition.y >= Chunk.height || chunkPosition.z >= Chunk.width || chunkPosition.x < 0 || chunkPosition.y < 0 || chunkPosition.z < 0)
            {
                Debug.Log("CԽ��");
                return 2;
            }

            // ��λ�����з���
            if (blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] != 0)
            {
                Debug.Log("C��λ�����з���");
                return 3;
            }
            Debug.Log("����");
            isWorking = true;
            mesh = new Mesh();
            mesh.name = "Chunk";
            blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] = 1;
            StartCoroutine(CreateMesh());
            return 0;
        }

        // ���ٷ���
        public byte DestroyBlock(Vector3 position)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);

            // ��������������
            if (chunkPosition.x >= Chunk.width || chunkPosition.y >= Chunk.height || chunkPosition.z >= Chunk.width || chunkPosition.x < 0 || chunkPosition.y < 0 || chunkPosition.z < 0)
            {
                Debug.Log("DԽ��");
                return 1;
            }

            // ��λ��û�з���
            if (blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] == 0)
            {
                Debug.Log("D��λ��û�з���");
                return 2;
            }
            Debug.Log(destroyTime);
            // �ж��Ƿ����ٹ�ʱ��
            if(destroyTime >= 0)
            {
                // ���������ʱ��
                destroyTime -= Time.deltaTime;
                return 3;
            }
            Debug.Log("����");
            isWorking = true;
            mesh = new Mesh();
            mesh.name = "Chunk";
            blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] = 0;
            StartCoroutine(CreateMesh());
            return 0;

        }

        // ˢ��ʱ��
        public void setDestroyTime(Vector3 position)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);

            Block block = BlockList.GetBlock(this.blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z]);
            if (block == null) return;
            destroyTime = block.destroyTime;
        }
    }
}