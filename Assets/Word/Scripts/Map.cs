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


        //当前是否正在生成Chunk
        private bool spawningChunk = false;

        void Awake()
        {
            instance = this;
            chunkPrefab = Resources.Load("Prefabs/Chunk") as GameObject;
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