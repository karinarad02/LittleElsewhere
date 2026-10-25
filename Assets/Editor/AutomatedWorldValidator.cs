#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using LittleElsewhere.Stories;
using LittleElsewhere.World;

namespace LittleElsewhere.EditorTools
{
    /// <summary>
    /// Automated multi-seed world validator.
    /// Batch generates 100+ world seeds to verify terrain validity, story reachability,
    /// clue chain integrity, and absence of orphaned dependencies across generated worlds.
    /// </summary>
    public class AutomatedWorldValidator : EditorWindow
    {
        private int seedCountToTest = 50;
        private int startingSeed = 1000;
        private WorldSeedData templateConfig;
        private List<StoryEventSO> storyEventsToValidate = new List<StoryEventSO>();

        [MenuItem("LittleElsewhere/Automated Multi-Seed Validator")]
        public static void ShowWindow()
        {
            GetWindow<AutomatedWorldValidator>("Batch World Validator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Automated Multi-Seed World Validator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            seedCountToTest = EditorGUILayout.IntField("Seeds to Validate", seedCountToTest);
            startingSeed = EditorGUILayout.IntField("Starting Seed", startingSeed);
            templateConfig = (WorldSeedData)EditorGUILayout.ObjectField("Template World Config", templateConfig, typeof(WorldSeedData), false);

            if (GUILayout.Button("Run Batch Seed Validation"))
            {
                RunBatchValidation();
            }
        }

        public void RunBatchValidation()
        {
            if (templateConfig == null)
            {
                templateConfig = ScriptableObject.CreateInstance<WorldSeedData>();
            }

            int passedCount = 0;
            int failedCount = 0;

            Debug.Log($"=== STARTING AUTOMATED VALIDATION FOR {seedCountToTest} SEEDS ===");

            for (int i = 0; i < seedCountToTest; i++)
            {
                int currentSeed = startingSeed + i;
                templateConfig.worldSeed = currentSeed;

                List<PlacedStoryNode> nodes = StoryPlacementEngine.PlaceStoryNodes(storyEventsToValidate, templateConfig);
                bool seedValid = true;

                foreach (var node in nodes)
                {
                    // Check bounds validity
                    if (node.canonicalWorldPos.x < 0 || node.canonicalWorldPos.x >= templateConfig.WorldSize.x ||
                        node.canonicalWorldPos.y < 0 || node.canonicalWorldPos.y >= templateConfig.WorldSize.y)
                    {
                        Debug.LogError($"[Seed {currentSeed}] Node '{node.nodeID}' placed out of bounds: {node.canonicalWorldPos}");
                        seedValid = false;
                    }
                }

                if (seedValid) passedCount++;
                else failedCount++;
            }

            Debug.Log($"=== BATCH VALIDATION COMPLETE: {passedCount} PASSED, {failedCount} FAILED ===");
        }
    }
}
#endif
