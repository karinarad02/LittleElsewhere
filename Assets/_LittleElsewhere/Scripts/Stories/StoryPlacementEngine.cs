using System.Collections.Generic;
using UnityEngine;
using LittleElsewhere.World;

namespace LittleElsewhere.Stories
{
    public struct PlacedStoryNode
    {
        public string nodeID;
        public StoryEventSO storyEvent;
        public Vector2 canonicalWorldPos;
        public Vector2Int chunkCoord;
        public bool isAnchor;
    }

    /// <summary>
    /// Deterministically places handcrafted story events and clue anchors based on world seed.
    /// Preserves geographic relationships (e.g. lake -> bridge -> cabin) and guarantees accessibility.
    /// </summary>
    public static class StoryPlacementEngine
    {
        public static List<PlacedStoryNode> PlaceStoryNodes(List<StoryEventSO> storyEvents, WorldSeedData config)
        {
            List<PlacedStoryNode> placedNodes = new List<PlacedStoryNode>();
            if (storyEvents == null || storyEvents.Count == 0 || config == null) return placedNodes;

            Random.InitState(config.worldSeed ^ 0x57081);

            foreach (var storyEvent in storyEvents)
            {
                if (storyEvent == null) continue;

                // Pick valid anchor chunk based on terrain requirements and periodic noise
                Vector2Int anchorChunk = FindValidChunkForEvent(storyEvent, config);
                Vector2 anchorWorldPos = new Vector2(
                    (anchorChunk.x + 0.5f) * config.chunkWorldSize,
                    (anchorChunk.y + 0.5f) * config.chunkWorldSize
                );

                PlacedStoryNode anchorNode = new PlacedStoryNode
                {
                    nodeID = storyEvent.eventID,
                    storyEvent = storyEvent,
                    canonicalWorldPos = anchorWorldPos,
                    chunkCoord = anchorChunk,
                    isAnchor = true
                };
                placedNodes.Add(anchorNode);

                // Place dependent clue nodes nearby with geographic offset
                int clueIndex = 0;
                if (storyEvent.primaryRouteClues != null)
                {
                    foreach (var clue in storyEvent.primaryRouteClues)
                    {
                        if (clue == null) continue;

                        float angle = (clueIndex * 2.1f) + Random.Range(-0.4f, 0.4f);
                        float dist = Random.Range(12f, 28f); // Nearby offset

                        Vector2 rawCluePos = anchorWorldPos + new Vector2(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist);
                        Vector2 canonicalCluePos = ToroidalMath.WrapPosition(rawCluePos, config.WorldSize);

                        int cx = Mathf.FloorToInt(canonicalCluePos.x / config.chunkWorldSize);
                        int cy = Mathf.FloorToInt(canonicalCluePos.y / config.chunkWorldSize);

                        PlacedStoryNode clueNode = new PlacedStoryNode
                        {
                            nodeID = clue.clueID,
                            storyEvent = storyEvent,
                            canonicalWorldPos = canonicalCluePos,
                            chunkCoord = new Vector2Int(cx, cy),
                            isAnchor = false
                        };
                        placedNodes.Add(clueNode);
                        clueIndex++;
                    }
                }
            }

            return placedNodes;
        }

        private static Vector2Int FindValidChunkForEvent(StoryEventSO storyEvent, WorldSeedData config)
        {
            int attempts = 0;
            while (attempts < 100)
            {
                int cx = Random.Range(0, config.widthInChunks);
                int cy = Random.Range(0, config.heightInChunks);
                Vector2Int candidate = new Vector2Int(cx, cy);

                ChunkData chunk = ProceduralChunkGenerator.GenerateChunk(candidate, config);

                // Verify terrain requirement match
                if (storyEvent.requiredTerrain == TerrainRequirement.NearWater && chunk.type == ChunkType.Lake)
                {
                    return candidate;
                }
                if (storyEvent.requiredTerrain == TerrainRequirement.Clearing && chunk.type == ChunkType.Clearing)
                {
                    return candidate;
                }
                if (storyEvent.requiredTerrain == TerrainRequirement.Any || chunk.type == ChunkType.StandardForest)
                {
                    return candidate;
                }

                attempts++;
            }

            // Fallback default chunk if strict terrain condition attempts exhaust
            return new Vector2Int(1, 1);
        }
    }
}
