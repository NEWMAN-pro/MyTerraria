using Soultia.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soultia.Voxel
{
    public class Map : MonoBehaviour
    {
        public static Map instance;

        // 区块
        public static GameObject chunkPrefab;

        // 区块队列
        public Dictionary<Vector3i, GameObject> chunks = new Dictionary<Vector3i, GameObject>();

        // 区块中方块信息队列
        public Dictionary<Vector3i, byte[,,]> chunkBlocks = new();

        // 玩家
        public static GameObject player;

        // 销毁方块
        public static GameObject destory;


        //当前是否正在生成Chunk
        private bool spawningChunk = false;

        void Awake()
        {
            instance = this;
            if (!StartUI.flag)
            {
                // 如果是继续游戏
                chunkBlocks = AccessGameAll.data.map;
            }
            chunkPrefab = Resources.Load("Prefabs/Chunk") as GameObject;
            player = Resources.Load("Prefabs/Player") as GameObject;
            player = Instantiate(player, new Vector3(0, 25, 0), Quaternion.identity);
            destory = Resources.Load("Prefabs/Destory") as GameObject;
            destory = Instantiate(destory, Vector3.zero, Quaternion.identity);
            player.GetComponent<PlayController>().destory = destory;
            StartCoroutine(SetTrue());
        }

        private void Update()
        {

        }

        // 恢复游戏
        IEnumerator SetTrue()
        {
            float startTime = Time.unscaledTime;
            while (Time.unscaledTime - startTime < 10)
            {
                yield return null;
            }
            this.transform.GetComponent<PauseGameAll>().UnPauseGame();
        }

        // 随玩家生成地图
        public void CreateMap(Vector3 position)
        {
            for (float x = position.x - Chunk.width * 3; x <= position.x + Chunk.width * 3; x += Chunk.width)
            {
                for (float y = position.y - Chunk.height * 3; y <= position.y + Chunk.height * 3; y += Chunk.height)
                {
                    //Y轴上是允许最大16个Chunk，方块高度最大是256
                    if (y <= Chunk.height * 16 && y >= -Chunk.height * 8)
                    {
                        for (float z = position.z - Chunk.width * 3; z <= position.z + Chunk.width * 3; z += Chunk.width)
                        {
                            int xx = Chunk.width * Mathf.FloorToInt(x / Chunk.width);
                            int yy = Chunk.height * Mathf.FloorToInt(y / Chunk.height);
                            int zz = Chunk.width * Mathf.FloorToInt(z / Chunk.width);
                            if (!ChunkExists(xx, yy, zz))
                            {
                                CreateChunk(new Vector3i(xx, yy, zz), false);
                            }
                            else
                            {
                                if(!chunks[new Vector3i(xx, yy, zz)].activeSelf)
                                {
                                    // 如果区块对象已生成且处于未激活状态，则激活区块
                                    CreateChunk(new Vector3i(xx, yy, zz), true);
                                }
                            }
                        }
                    }
                }
            }
        }

        //生成Chunk
        public void CreateChunk(Vector3i pos, bool flag)
        {
            if (spawningChunk) return;

            StartCoroutine(SpawnChunk(pos, flag));
        }

        private IEnumerator SpawnChunk(Vector3i pos, bool flag)
        {
            spawningChunk = true;
            if (flag)
            {
                // 已存在的区块直接显示出来即可
                chunks[pos].SetActive(true);
            }
            else
            {
                GameObject chunk = Instantiate(chunkPrefab, pos, Quaternion.identity);
                if (chunkBlocks.ContainsKey(pos))
                {
                    // 如果存档中改区块有记录，则加载
                    chunk.GetComponent<Chunk>().blocks = chunkBlocks[pos];
                    chunk.GetComponent<Chunk>().isFinished = true;
                }
            }
            yield return null;
            spawningChunk = false;
        }

        //通过Chunk的坐标来判断它是否存在
        public bool ChunkExists(Vector3i worldPosition)
        {
            return this.ChunkExists(worldPosition.x, worldPosition.y, worldPosition.z);
        }
        //通过Chunk的坐标来判断它是否存在
        public bool ChunkExists(int x, int y, int z)
        {
            return chunks.ContainsKey(new Vector3i(x, y, z));
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}