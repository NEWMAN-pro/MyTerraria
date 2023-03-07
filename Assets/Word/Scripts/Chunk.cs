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
        // ���������
        public float fluctuation = 4f;

        // �ϴ�ѡ�����ٵķ���
        private Vector3i lastBlock = Vector3i.back;
        // ��������ʱ��
        private float destroyTime;
        // ����Ѫ��
        private float blockHP;
        // �Ƿ�����
        public bool destroy = false;

        // ���������
        public int seed = 1234;
        // ��ľ���ܶ�
        public int treeDensity = 10;

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
                // �����������MAP�����
                this.transform.transform.SetParent(Map.instance.transform);
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

        // ����
        IEnumerator CreateMap()
        {
            blocks = new byte[width, height, width];
            for (int x = 0; x < Chunk.width; x++)
            {
                for (int y = 0; y < Chunk.height; y++)
                {
                    for (int z = 0; z < Chunk.width; z++)
                    {
                        if(position.y >= 0)
                        {
                            if(position.x <= 48 && position.x >= -48 && position.z <= 48 && position.z >= -48)
                            {
                                // ��ʼΪƽԭ
                                fluctuation = 4f;
                            }
                            else
                            {
                                // �����ĵ�ԽԶ�����򣬵������Խ��
                                fluctuation = Math.Max(0.5f, 4f - (Math.Abs(Vector2.Distance(new Vector2(position.x, position.z), Vector2.zero)) - 48) / 128);
                            }
                            byte blockid = Terrain.GetTerrainBlock(new Vector3i(x, y, z) + position, fluctuation);
                            byte upBlockid = Terrain.GetTerrainBlock(new Vector3i(x, y + 1, z) + position, fluctuation);
                            if (position.y == 0 && y == 0 && upBlockid == 0)
                            {
                                // ���ݿն��òݷ����
                                blocks[x, y, z] = 2;
                            }
                            else if (blockid == 2)
                            {
                                if (upBlockid == 0)
                                {
                                    // ��������ϲ�û�з��飬���òݷ���
                                    blocks[x, y, z] = 2;
                                }
                                else
                                {
                                    // ������ʯ��
                                    blocks[x, y, z] = 3;
                                }
                            }
                            else if (blockid == 1 && upBlockid == 0)
                            {
                                // ���ݷ���
                                blocks[x, y, z] = 2;
                                CreateTree(x, y, z);
                            }
                            else
                            {
                                if(blocks[x, y, z] == 0) blocks[x, y, z] = blockid;
                            }
                        }
                        else
                        {
                            if(position.y <= -128 && y <= 8)
                            {
                                // ��ײ����ɻ���
                                blocks[x, y, z] = 5;
                            }
                            else
                            {
                                // �ײ�ȫ����ʯͷ
                                blocks[x, y, z] = 3;
                                //int voidFlag = Random(10);
                                //if (voidFlag == 0)
                                //{
                                //    blocks[x, y, z] = 0;
                                //}
                            }
                        }
                    }
                }
            }

            yield return null;
            StartCoroutine(CreateMesh());
        }

        // ����
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

        // ����һ����
        public void CreateTree(int x, int y, int z)
        {
            int treeFlag = Random(treeDensity);
            if (treeFlag == 0 && y < 10 && x > 0 && x < 15 && z > 0 && z < 15)
            {
                for(int i = 0; i < 3; i++)
                {
                    for(int j = 0; j < 3; j++)
                    {
                        if(blocks[x - 1 + i, y, z - 1 + j] == 6)
                        {
                            // ����Ź�������һ����������
                            return;
                        }
                    }
                }
                // ��������
                for (int i = 1; i <= 5; i++)
                {
                    blocks[x, y + i, z] = 6;
                }
                // ������Ҷ
                blocks[x, y + 6, z] = 7;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (!(i == j && i == 1))
                        {
                            if (blocks[x - 1 + i, y + 6, z - 1 + j] == 0) blocks[x - 1 + i, y + 6, z - 1 + j] = 7;
                            if (blocks[x - 1 + i, y + 5, z - 1 + j] == 0) blocks[x - 1 + i, y + 5, z - 1 + j] = 7;
                            if (blocks[x - 1 + i, y + 4, z - 1 + j] == 0) blocks[x - 1 + i, y + 4, z - 1 + j] = 7;
                        }
                    }
                }
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

            //if (destroy)
            //{
            //    AddDestroyFace(x, y, z);
            //    //���4����
            //    vertices.Add(new Vector3(0 + x, 1 + y, 0 + z));
            //    vertices.Add(new Vector3(0 + x, 1 + y, 1 + z));
            //    vertices.Add(new Vector3(-1 + x, 1 + y, 1 + z));
            //    vertices.Add(new Vector3(-1 + x, 1 + y, 0 + z));
            //}
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

        // ����
        void AddDestroyFace(int x, int y, int z)
        {
            //��һ��������
            triangles.Add(1 + vertices.Count);
            triangles.Add(0 + vertices.Count);
            triangles.Add(3 + vertices.Count);

            //�ڶ���������
            triangles.Add(3 + vertices.Count);
            triangles.Add(2 + vertices.Count);
            triangles.Add(1 + vertices.Count);

            //���UV����㣬������4����ѭ����˳��һ��
            float flag = (float)Math.Floor(blockHP / 10);
            uv.Add(new Vector2(flag * textureOffset, 16 * textureOffset) + new Vector2(shrinkSize, shrinkSize));
            uv.Add(new Vector2(flag * textureOffset + textureOffset, 16 * textureOffset) + new Vector2(-shrinkSize, shrinkSize));
            uv.Add(new Vector2(flag * textureOffset + textureOffset, 16 * textureOffset + textureOffset) + new Vector2(-shrinkSize, -shrinkSize));
            uv.Add(new Vector2(flag * textureOffset, 16 * textureOffset + textureOffset) + new Vector2(shrinkSize, -shrinkSize));
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
        public byte CreateBlock(Vector3 position, Vector3 playPosition, byte blockID)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);
            Vector3 wordPosition = ChunkTransferWorld(chunkPosition);

            // ������ɫ�غϣ�������
            if(Math.Abs(wordPosition.x - playPosition.x) < 0.5f && Math.Abs(wordPosition.y - playPosition.y) < 1f && Math.Abs(wordPosition.z - playPosition.z) < 0.5f)
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
            //Debug.Log("����");
            isWorking = true;
            mesh = new Mesh();
            mesh.name = "Chunk";
            blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] = blockID;
            if(blockID == 8)
            {
                // ����Ǳ��䣬���뱦����У�keyΪ��������ӷ�������
                string key = this.transform.name + chunkPosition.ToString();
                BoxList.AddBox(key);
            }
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

            // �����λ���Ǳ����ұ��䲻Ϊ��
            if(blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] == 8 && BoxList.GetBox(this.transform.name + chunkPosition.ToString()).Count != 0)
            {
                Debug.Log("D�ñ��䲻Ϊ��");
                return 4;
            }

            // �������ı䣬�������Ϣ
            if(chunkPosition != lastBlock)
            {
                lastBlock = chunkPosition;
                SetDestroyTime(position);
            }
            //Debug.Log(blockHP);
            isWorking = true;
            mesh = new Mesh();
            mesh.name = "Chunk";
            // �ж��Ƿ����ٹ�ʱ��
            if(blockHP <= 100)
            {
                destroy = true;
                // ���������ʱ��
                blockHP += Time.deltaTime * destroyTime;
                //StartCoroutine(CreateMesh());
                destroy = false;
                return 3;
            }
            //Debug.Log("����");
            if(blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] == 8)
            {
                // ������ٵķ����Ǳ��䣬�򽫸ñ���ӱ����б����Ƴ�
                BoxList.DelectBox(this.transform.name + chunkPosition.ToString());
            }
            blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] = 0;
            StartCoroutine(CreateMesh());
            return 0;

        }

        // ˢ��ʱ��
        public void SetDestroyTime(Vector3 position)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);

            Block block = BlockList.GetBlock(this.blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z]);
            if (block == null) return;
            destroyTime = 100 / block.destroyTime;
            blockHP = 0;
        }

        // �жϵ�ǰλ���Ƿ��Ǳ���
        public string GetBox(Vector3 position)
        {
            Vector3i chunkPosition = WorldTransferChunk(position);
            if (blocks[chunkPosition.x, chunkPosition.y, chunkPosition.z] == 8)
            {
                return this.transform.name + chunkPosition.ToString();
            }
            return "_";
        }

        // ���������
        public int Random(int max)
        {
            System.Random rand = new System.Random(seed);
            int flag = rand.Next(0, max);
            seed = seed + flag + 1;
            return flag;
        }
    }
}