using UnityEngine;

namespace LittleElsewhere.World
{
    /// <summary>
    /// Debug view that visualizes world boundaries, chunk grids, and canonical player position in Editor/Scene view.
    /// </summary>
    public class WorldDebugGizmos : MonoBehaviour
    {
        [SerializeField] private WorldSeedData config;
        [SerializeField] private bool showWorldBounds = true;
        [SerializeField] private bool showChunkGrid = true;
        [SerializeField] private Color worldBoundaryColor = Color.cyan;
        [SerializeField] private Color chunkGridColor = new Color(1f, 1f, 1f, 0.25f);

        private void OnDrawGizmos()
        {
            if (config == null) return;

            Vector2 worldSize = config.WorldSize;

            if (showWorldBounds)
            {
                Gizmos.color = worldBoundaryColor;
                Vector3 center = new Vector3(worldSize.x * 0.5f, worldSize.y * 0.5f, 0f);
                Vector3 size = new Vector3(worldSize.x, worldSize.y, 1f);
                Gizmos.DrawWireCube(center, size);
            }

            if (showChunkGrid)
            {
                Gizmos.color = chunkGridColor;
                float cs = config.chunkWorldSize;

                for (int x = 0; x <= config.widthInChunks; x++)
                {
                    Gizmos.DrawLine(new Vector3(x * cs, 0f, 0f), new Vector3(x * cs, worldSize.y, 0f));
                }

                for (int y = 0; y <= config.heightInChunks; y++)
                {
                    Gizmos.DrawLine(new Vector3(0f, y * cs, 0f), new Vector3(worldSize.x, y * cs, 0f));
                }
            }
        }
    }
}
