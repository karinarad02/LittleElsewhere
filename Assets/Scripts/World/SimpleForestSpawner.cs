using UnityEngine;

namespace LittleElsewhere.World
{
    /// <summary>
    /// Prototype forest spawner that deterministically generates pine trees and clearings based on a seed.
    /// Used for initial visual prototype testing of forest density, negative space, and tree distribution.
    /// </summary>
    public class SimpleForestSpawner : MonoBehaviour
    {
        [Header("World Generation Seed")]
        [SerializeField, Tooltip("Random seed for deterministic forest placement.")]
        private int worldSeed = 1337;

        [Header("Spawn Bounds")]
        [SerializeField, Tooltip("Width and height of the prototype forest grid area.")]
        private Vector2Int forestAreaSize = new Vector2Int(50, 50);

        [SerializeField, Tooltip("Minimum distance between pine tree instances.")]
        private float minimumTreeDistance = 2.5f;

        [Header("Tree Prefabs & Foliage")]
        [SerializeField, Tooltip("Array of pine tree prefabs to instantiate.")]
        private GameObject[] pineTreePrefabs;

        [SerializeField, Range(0.1f, 0.9f), Tooltip("Perlin noise threshold for forest tree density.")]
        private float treeDensityThreshold = 0.45f;

        [SerializeField, Tooltip("Scale factor for Perlin noise feature size.")]
        private float noiseScale = 0.08f;

        private void Start()
        {
            GenerateForest();
        }

        [ContextMenu("Regenerate Forest")]
        public void GenerateForest()
        {
            // Clear existing spawned tree children if any
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(transform.GetChild(i).gameObject);
            }

            if (pineTreePrefabs == null || pineTreePrefabs.Length == 0)
            {
                Debug.LogWarning("[SimpleForestSpawner] No pine tree prefabs assigned.");
                return;
            }

            Random.InitState(worldSeed);
            float offsetX = Random.Range(-10000f, 10000f);
            float offsetY = Random.Range(-10000f, 10000f);

            int halfWidth = forestAreaSize.x / 2;
            int halfHeight = forestAreaSize.y / 2;

            for (float x = -halfWidth; x < halfWidth; x += minimumTreeDistance)
            {
                for (float y = -halfHeight; y < halfHeight; y += minimumTreeDistance)
                {
                    // Leave a small clearing around center (0,0) for player starting area
                    if (Mathf.Abs(x) < 4f && Mathf.Abs(y) < 4f) continue;

                    float sampleX = (x + offsetX) * noiseScale;
                    float sampleY = (y + offsetY) * noiseScale;
                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

                    if (noiseValue > treeDensityThreshold)
                    {
                        // Add slight random jitter to tree position for organic look
                        Vector3 position = new Vector3(
                            x + Random.Range(-0.4f, 0.4f),
                            y + Random.Range(-0.4f, 0.4f),
                            0f
                        );

                        GameObject treePrefab = pineTreePrefabs[Random.Range(0, pineTreePrefabs.Length)];
                        GameObject treeInstance = Instantiate(treePrefab, position, Quaternion.identity, transform);

                        // Randomize slight scale variation for visual richness
                        float scaleJitter = Random.Range(0.85f, 1.15f);
                        treeInstance.transform.localScale = Vector3.one * scaleJitter;
                    }
                }
            }

            Debug.Log($"[SimpleForestSpawner] Forest successfully generated with seed {worldSeed}.");
        }
    }
}
