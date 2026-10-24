using System.Collections.Generic;
using UnityEngine;

namespace LittleElsewhere.World
{
    /// <summary>
    /// Manages active procedural chunk streaming and ghost rendering for wraparound edges.
    /// Ensures objects near world boundaries render seamlessly on opposite edges.
    /// </summary>
    public class ChunkManager : MonoBehaviour
    {
        [Header("World Data Configuration")]
        [SerializeField] private WorldSeedData worldConfig;

        [Header("Streaming Settings")]
        [SerializeField, Tooltip("Load radius in chunk units around player.")]
        private int viewDistanceChunks = 2;

        [SerializeField, Tooltip("Target transform to follow (Player).")]
        private Transform playerTransform;

        [Header("Prefabs")]
        [SerializeField] private GameObject[] pineTreePrefabs;
        [SerializeField] private GameObject snowChunkPrefab;

        private Dictionary<Vector2Int, GameObject> loadedChunkInstances = new Dictionary<Vector2Int, GameObject>();
        private Vector2Int currentCenterChunk;

        public WorldSeedData WorldConfig => worldConfig;

        private void Start()
        {
            if (worldConfig == null)
            {
                worldConfig = ScriptableObject.CreateInstance<WorldSeedData>();
            }

            if (playerTransform == null)
            {
                GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null) playerTransform = playerGO.transform;
            }

            UpdateChunkStreaming(true);
        }

        private void Update()
        {
            if (playerTransform == null) return;

            Vector2 canonicalPlayerPos = ToroidalMath.WrapPosition(playerTransform.position, worldConfig.WorldSize);
            Vector2Int playerChunk = GetChunkCoord(canonicalPlayerPos);

            if (playerChunk != currentCenterChunk)
            {
                currentCenterChunk = playerChunk;
                UpdateChunkStreaming(false);
            }
        }

        public Vector2Int GetChunkCoord(Vector2 worldPos)
        {
            int cx = Mathf.FloorToInt(worldPos.x / worldConfig.chunkWorldSize);
            int cy = Mathf.FloorToInt(worldPos.y / worldConfig.chunkWorldSize);
            return new Vector2Int(cx, cy);
        }

        private void UpdateChunkStreaming(bool forceRebuild)
        {
            HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();

            for (int dx = -viewDistanceChunks; dx <= viewDistanceChunks; dx++)
            {
                for (int dy = -viewDistanceChunks; dy <= viewDistanceChunks; dy++)
                {
                    Vector2Int rawChunk = new Vector2Int(currentCenterChunk.x + dx, currentCenterChunk.y + dy);
                    requiredChunks.Add(rawChunk);
                }
            }

            // Unload distant chunks
            List<Vector2Int> toUnload = new List<Vector2Int>();
            foreach (var kvp in loadedChunkInstances)
            {
                if (!requiredChunks.Contains(kvp.Key) || forceRebuild)
                {
                    toUnload.Add(kvp.Key);
                }
            }

            foreach (var coord in toUnload)
            {
                if (loadedChunkInstances.TryGetValue(coord, out GameObject go))
                {
                    Destroy(go);
                    loadedChunkInstances.Remove(coord);
                }
            }

            // Load new chunks
            foreach (var coord in requiredChunks)
            {
                if (!loadedChunkInstances.ContainsKey(coord))
                {
                    GameObject chunkGO = InstantiateChunk(coord);
                    loadedChunkInstances.Add(coord, chunkGO);
                }
            }
        }

        private GameObject InstantiateChunk(Vector2Int virtualChunkCoord)
        {
            // Wrap to canonical chunk coord
            int wrappedX = (virtualChunkCoord.x % worldConfig.widthInChunks + worldConfig.widthInChunks) % worldConfig.widthInChunks;
            int wrappedY = (virtualChunkCoord.y % worldConfig.heightInChunks + worldConfig.heightInChunks) % worldConfig.heightInChunks;

            ChunkData data = ProceduralChunkGenerator.GenerateChunk(new Vector2Int(wrappedX, wrappedY), worldConfig);

            // Render position based on virtual chunk coordinate (supporting wraparound ghost offset)
            Vector3 worldPos = new Vector3(virtualChunkCoord.x * worldConfig.chunkWorldSize, virtualChunkCoord.y * worldConfig.chunkWorldSize, 0f);

            GameObject chunkGO = new GameObject($"Chunk_{virtualChunkCoord.x}_{virtualChunkCoord.y}");
            chunkGO.transform.position = worldPos;
            chunkGO.transform.parent = transform;

            // Spawn trees
            if (pineTreePrefabs != null && pineTreePrefabs.Length > 0)
            {
                foreach (var tree in data.trees)
                {
                    int index = Mathf.Clamp(tree.prefabIndex, 0, pineTreePrefabs.Length - 1);
                    Vector3 treePos = worldPos + (Vector3)tree.localPosition;
                    GameObject treeInst = Instantiate(pineTreePrefabs[index], treePos, Quaternion.identity, chunkGO.transform);
                    treeInst.transform.localScale = Vector3.one * tree.scale;
                }
            }

            return chunkGO;
        }
    }
}
