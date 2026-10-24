using UnityEngine;

namespace LittleElsewhere.Player
{
    /// <summary>
    /// Spawns footprints in snow behind the player character at distance intervals.
    /// Manages alternating left/right foot placement, rotation matching movement direction, and alpha fade decay.
    /// </summary>
    public class FootprintSpawner : MonoBehaviour
    {
        [Header("Footprint Settings")]
        [SerializeField, Tooltip("Footprint sprite prefab with SpriteRenderer and FootprintDecay.")]
        private GameObject footprintPrefab;

        [SerializeField, Tooltip("Distance interval between steps in world units.")]
        private float stepDistance = 0.5f;

        [SerializeField, Tooltip("Lateral offset for left and right foot spacing.")]
        private float footOffset = 0.12f;

        [SerializeField, Tooltip("Lifetime of footprint in seconds before disappearing.")]
        private float footprintLifetime = 18.0f;

        [SerializeField, Tooltip("Sorting layer name for footprint rendering behind character.")]
        private string footprintSortingLayer = "Footprints";

        [Header("References")]
        [SerializeField]
        private PlayerController playerController;

        private Vector3 lastStepPosition;
        private bool isLeftFoot = true;

        private void Start()
        {
            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }
            lastStepPosition = transform.position;
        }

        private void Update()
        {
            if (playerController == null || !playerController.IsMoving) return;

            float distanceMoved = Vector3.Distance(transform.position, lastStepPosition);
            if (distanceMoved >= stepDistance)
            {
                SpawnFootprint();
                lastStepPosition = transform.position;
            }
        }

        private void SpawnFootprint()
        {
            if (footprintPrefab == null) return;

            Vector2 moveDir = playerController.CurrentVelocity.normalized;
            if (moveDir.sqrMagnitude < 0.01f) return;

            // Calculate perpendicular vector for left/right offset
            Vector2 perpDir = new Vector2(-moveDir.y, moveDir.x);
            float sideMultiplier = isLeftFoot ? -1.0f : 1.0f;
            isLeftFoot = !isLeftFoot;

            Vector3 spawnPos = transform.position + (Vector3)(perpDir * (footOffset * sideMultiplier));
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject footprintGO = Instantiate(footprintPrefab, spawnPos, rotation);

            // Configure sorting layer if SpriteRenderer exists
            SpriteRenderer sr = footprintGO.GetComponent<SpriteRenderer>();
            if (sr != null && !string.IsNullOrEmpty(footprintSortingLayer))
            {
                sr.sortingLayerName = footprintSortingLayer;
            }

            FootprintDecay decay = footprintGO.GetComponent<FootprintDecay>();
            if (decay == null)
            {
                decay = footprintGO.AddComponent<FootprintDecay>();
            }
            decay.Initialize(footprintLifetime);
        }
    }

    /// <summary>
    /// Smoothly fades out footprint sprite alpha over its lifetime and handles garbage cleanup.
    /// </summary>
    public class FootprintDecay : MonoBehaviour
    {
        private float lifetime = 18.0f;
        private float timer = 0.0f;
        private SpriteRenderer spriteRenderer;
        private Color initialColor;

        public void Initialize(float duration)
        {
            lifetime = duration;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                initialColor = spriteRenderer.color;
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            float progress = timer / lifetime;

            if (spriteRenderer != null)
            {
                // Smooth fade during the last 50% of footprint lifespan
                float alphaMultiplier = Mathf.Clamp01((1.0f - progress) / 0.5f);
                Color c = initialColor;
                c.a = initialColor.a * alphaMultiplier;
                spriteRenderer.color = c;
            }

            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
