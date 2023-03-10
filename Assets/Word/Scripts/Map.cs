using Soultia.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soultia.Voxel
{
    public class Map : MonoBehaviour
    {
        public static Map instance;

        public static GameObject chunkPrefab;

        public Dictionary<Vector3i, GameObject> chunks = new Dictionary<Vector3i, GameObject>();

        public GameObject player = null;


        //当前是否正在生成Chunk
        private bool spawningChunk = false;

        void Awake()
        {
            instance = this;
            chunkPrefab = Resources.Load("Prefabs/Chunk") as GameObject;
            player = Resources.Load("Prefabs/Player") as GameObject;
            player = Instantiate(player, new Vector3(0, 25, 0), Quaternion.identity);
            // 暂停角色
            StartCoroutine(SetTrue());
        }

        private void Update()
        {

        }

        // 恢复游戏
        IEnumerator SetTrue()
        {
            yield return new WaitForSeconds(5);
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
                                CreateChunk(new Vector3i(xx, yy, zz));
                            }
                            else
                            {
                                // 已存在的区块直接显示出来即可
                                chunks[new Vector3i(xx, yy, zz)].SetActive(true);
                            }
                        }
                    }
                }
            }
            //if(cnt <= 600)
            //{
            //    cnt++;
            //}
        }

        //生成Chunk
        public void CreateChunk(Vector3i pos)
        {
            if (spawningChunk) return;

            StartCoroutine(SpawnChunk(pos));
        }

        private IEnumerator SpawnChunk(Vector3i pos)
        {
            spawningChunk = true;
            Instantiate(chunkPrefab, pos, Quaternion.identity);
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
    }
}