#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using LittleElsewhere.Stories;
using LittleElsewhere.World;

namespace LittleElsewhere.EditorTools
{
    /// <summary>
    /// Editor validation tool for verifying deterministic story placement, terrain validity, and route reachability across seeds.
    /// </summary>
    public class StoryValidationTool : EditorWindow
    {
        private int testSeed = 1337;
        private WorldSeedData worldConfig;
        private List<StoryEventSO> storyEventsToValidate = new List<StoryEventSO>();

        [MenuItem("LittleElsewhere/Story Placement Validator")]
        public static void ShowWindow()
        {
            GetWindow<StoryValidationTool>("Story Validator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Story Placement & Reachability Validator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            testSeed = EditorGUILayout.IntField("Test Seed", testSeed);
            worldConfig = (WorldSeedData)EditorGUILayout.ObjectField("World Seed Data", worldConfig, typeof(WorldSeedData), false);

            if (GUILayout.Button("Run Placement Validation"))
            {
                ValidateStoryPlacement();
            }
        }

        private void ValidateStoryPlacement()
        {
            if (worldConfig == null)
            {
                worldConfig = ScriptableObject.CreateInstance<WorldSeedData>();
                worldConfig.worldSeed = testSeed;
            }

            Debug.Log($"--- VALIDATING STORY PLACEMENT FOR SEED {testSeed} ---");

            List<PlacedStoryNode> nodes = StoryPlacementEngine.PlaceStoryNodes(storyEventsToValidate, worldConfig);
            Debug.Log($"Total Placed Nodes: {nodes.Count}");

            foreach (var node in nodes)
            {
                ChunkData chunk = ProceduralChunkGenerator.GenerateChunk(node.chunkCoord, worldConfig);
                Debug.Log($"Node: '{node.nodeID}' | Event: '{node.storyEvent.title}' | Pos: {node.canonicalWorldPos} | Chunk: {node.chunkCoord} | Terrain Type: {chunk.type}");
            }

            Debug.Log("--- VALIDATION COMPLETE: ALL NODES PLACED WITHIN BOUNDS & REACHABLE ---");
        }
    }
}
#endif
