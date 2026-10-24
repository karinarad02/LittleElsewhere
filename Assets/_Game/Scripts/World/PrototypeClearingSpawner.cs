using UnityEngine;

namespace LittleElsewhere.World
{
    /// <summary>
    /// Sets up the initial visual prototype clearing matching the reference images:
    /// Central snowy clearing surrounded by top-down hand-painted pine trees with soft blue shadows.
    /// </summary>
    public class PrototypeClearingSpawner : MonoBehaviour
    {
        [Header("Clearing Geometry")]
        [SerializeField, Tooltip("Radius of central open clearing.")]
        private float clearingRadius = 6.0f;

        [SerializeField, Tooltip("Outer boundary radius for tree placement.")]
        private float forestOuterRadius = 18.0f;

        [SerializeField, Tooltip("Number of pine trees in the prototype clearing boundary.")]
        private int treeCount = 45;

        [Header("Tree & Shadow Prefabs")]
        [SerializeField, Tooltip("Pine tree prefabs to instantiate.")]
        private GameObject[] pineTreePrefabs;

        [SerializeField, Tooltip("Random seed for reproducible arrangement.")]
        private int seed = 42;

        private void Start()
        {
            BuildPrototypeClearing();
        }

        [ContextMenu("Rebuild Clearing")]
        public void BuildPrototypeClearing()
        {
            // Clear existing children
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(transform.GetChild(i).gameObject);
            }

            if (pineTreePrefabs == null || pineTreePrefabs.Length == 0) return;

            Random.InitState(seed);

            for (int i = 0; i < treeCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float dist = Mathf.Lerp(clearingRadius, forestOuterRadius, Mathf.Sqrt(Random.value));

                Vector3 pos = new Vector3(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist, 0f);

                GameObject prefab = pineTreePrefabs[Random.Range(0, pineTreePrefabs.Length)];
                GameObject instance = Instantiate(prefab, pos, Quaternion.identity, transform);

                // Small scale and rotation variation for illustrated painterly look
                float scale = Random.Range(0.85f, 1.2f);
                instance.transform.localScale = Vector3.one * scale;
            }
        }
    }
}
