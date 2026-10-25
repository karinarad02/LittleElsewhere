using System.Collections.Generic;
using UnityEngine;

namespace LittleElsewhere.World
{
    public struct TreeData
    {
        public Vector2 localPosition;
        public Vector2 canonicalWorldPosition;
        public float scale;
        public int prefabIndex;
    }

    public enum ChunkType
    {
        StandardForest,
        Clearing,
        Lake,
        Trail
    }

    public class ChunkData
    {
        public Vector2Int chunkCoord;
        public ChunkType type;
        public List<TreeData> trees = new List<TreeData>();
    }

    /// <summary>
    /// Deterministic chunk generator. Calculates chunk terrain types and tree placement
    /// based on world seed and canonical chunk coordinate.
    /// </summary>
    public static class ProceduralChunkGenerator
    {
        public static ChunkData GenerateChunk(Vector2Int chunkCoord, WorldSeedData config)
        {
            // Wrap chunk coordinate modulo world bounds
            int wrappedX = (chunkCoord.x % config.widthInChunks + config.widthInChunks) % config.widthInChunks;
            int wrappedY = (chunkCoord.y % config.heightInChunks + config.heightInChunks) % config.heightInChunks;

            Vector2 chunkWorldOrigin = new Vector2(wrappedX * config.chunkWorldSize, wrappedY * config.chunkWorldSize);
            Vector2 worldSize = config.WorldSize;

            ChunkData chunk = new ChunkData();
            chunk.chunkCoord = new Vector2Int(wrappedX, wrappedY);

            // Sample center periodic noise to determine chunk primary type
            Vector2 chunkCenter = chunkWorldOrigin + new Vector2(config.chunkWorldSize * 0.5f, config.chunkWorldSize * 0.5f);
            float centerNoise = ToroidalMath.SamplePeriodicNoise(chunkCenter.x, chunkCenter.y, worldSize, config.noiseScale, config.worldSeed);

            if (centerNoise < config.lakeThreshold)
            {
                chunk.type = ChunkType.Lake;
            }
            else if (centerNoise > config.clearingThreshold)
            {
                chunk.type = ChunkType.Clearing;
            }
            else
            {
                chunk.type = ChunkType.StandardForest;
            }

            // Generate trees deterministically using seed combined with wrapped chunk coord
            int chunkSeed = config.worldSeed ^ (wrappedX * 73856093) ^ (wrappedY * 19349663);
            Random.InitState(chunkSeed);

            if (chunk.type == ChunkType.StandardForest)
            {
                float step = 2.5f;
                for (float lx = 1.0f; lx < config.chunkWorldSize - 1.0f; lx += step)
                {
                    for (float ly = 1.0f; ly < config.chunkWorldSize - 1.0f; ly += step)
                    {
                        Vector2 canonicalPos = chunkWorldOrigin + new Vector2(lx, ly);
                        float n = ToroidalMath.SamplePeriodicNoise(canonicalPos.x, canonicalPos.y, worldSize, config.noiseScale * 2f, config.worldSeed);

                        if (n > config.treeDensityThreshold)
                        {
                            TreeData tree = new TreeData
                            {
                                localPosition = new Vector2(lx + Random.Range(-0.3f, 0.3f), ly + Random.Range(-0.3f, 0.3f)),
                                canonicalWorldPosition = canonicalPos,
                                scale = Random.Range(0.85f, 1.15f),
                                prefabIndex = Random.Range(0, 3)
                            };
                            chunk.trees.Add(tree);
                        }
                    }
                }
            }

            return chunk;
        }
    }
}
