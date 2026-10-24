using UnityEngine;

namespace LittleElsewhere.World
{
    /// <summary>
    /// Data configuration for seed-based procedural world generation.
    /// Defines world dimensions in chunk units, chunk size, and noise density thresholds.
    /// </summary>
    [CreateAssetMenu(fileName = "WorldSeedData", menuName = "LittleElsewhere/World Seed Data")]
    public class WorldSeedData : ScriptableObject
    {
        [Header("Seed Configuration")]
        [SerializeField, Tooltip("Primary world generation seed.")]
        public int worldSeed = 1337;

        [Header("World Dimensions")]
        [SerializeField, Tooltip("Number of chunks along X axis.")]
        public int widthInChunks = 10;

        [SerializeField, Tooltip("Number of chunks along Y axis.")]
        public int heightInChunks = 10;

        [SerializeField, Tooltip("Width and height of each chunk in world units.")]
        public float chunkWorldSize = 16.0f;

        [Header("Procedural Feature Density")]
        [SerializeField, Tooltip("Base scale for periodic noise sampling.")]
        public float noiseScale = 1.2f;

        [SerializeField, Range(0.1f, 0.9f), Tooltip("Tree placement density threshold.")]
        public float treeDensityThreshold = 0.48f;

        [SerializeField, Range(0.0f, 0.5f), Tooltip("Lake and water body threshold.")]
        public float lakeThreshold = 0.22f;

        [SerializeField, Range(0.0f, 0.5f), Tooltip("Clearing threshold.")]
        public float clearingThreshold = 0.75f;

        public Vector2 WorldSize => new Vector2(widthInChunks * chunkWorldSize, heightInChunks * chunkWorldSize);
    }
}
