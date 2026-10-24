using UnityEngine;

namespace LittleElsewhere.Player
{
    /// <summary>
    /// Spawns footprints in snow behind the player character at set distance intervals.
    /// Manages alternating left/right foot placement, rotation matching movement direction, and fade-out lifetime.
    /// </summary>
    public class FootprintSpawner : MonoBehaviour
    {
        [Header("Footprint Configuration")]
        [SerializeField, Tooltip("Footprint prefab with SpriteRenderer and optional FootprintDecay script.")]
        private GameObject footprintPrefab;

        [SerializeField, Tooltip("Distance interval between footprints in world units.")]
        private float stepDistance = 0.6f;

        [SerializeField, Tooltip("Lateral distance offset for left and right feet from centerline.")]
        private float footOffset = 0.15f;

        [SerializeField, Tooltip("Lifetime of footprints in seconds before disappearing.")]
        private float footprintLifetime = 15.0f;

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
            isLeftFoot = !isLeftFoot; // Alternate feet

            Vector3 spawnPos = transform.position + (Vector3)(perpDir * (footOffset * sideMultiplier));
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject footprintGO = Instantiate(footprintPrefab, spawnPos, rotation);

            // Add or configure auto-destroy decay script
            FootprintDecay decay = footprintGO.GetComponent<FootprintDecay>();
            if (decay == null)
            {
                decay = footprintGO.AddComponent<FootprintDecay>();
            }
            decay.Initialize(footprintLifetime);
        }
    }

    /// <summary>
    /// Fades footprint sprite alpha over its lifetime and destroys the object upon expiration.
    /// </summary>
    public class FootprintDecay : MonoBehaviour
    {
        private float lifetime = 15.0f;
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
                // Gradually fade out in the last 40% of lifetime
                float alphaProgress = Mathf.Clamp01((1.0f - progress) / 0.4f);
                Color c = initialColor;
                c.a = initialColor.a * alphaProgress;
                spriteRenderer.color = c;
            }

            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
