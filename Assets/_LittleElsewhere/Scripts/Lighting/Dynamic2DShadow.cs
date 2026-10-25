using UnityEngine;

namespace LittleElsewhere.Lighting
{
    /// <summary>
    /// Dynamically casts and updates 2D top-down ground shadows for trees, campsite props, and the player character.
    /// Shadows rotate, stretch, compress, and adjust opacity continuously according to the 24-hour sun position cycle.
    /// Matches the soft cool-blue winter shadow aesthetic from reference artwork.
    /// </summary>
    [DisallowMultipleComponent]
    public class Dynamic2DShadow : MonoBehaviour
    {
        [Header("Shadow Visual Tuning")]
        [SerializeField, Tooltip("Soft blue shadow tint color matching watercolor reference style.")]
        private Color shadowColor = new Color(0.23f, 0.33f, 0.50f, 0.45f);

        [SerializeField, Tooltip("Shadow offset relative to ground pivot.")]
        private Vector3 baseShadowOffset = new Vector3(0f, -0.15f, 0f);

        [SerializeField, Tooltip("Base scale multiplier for shadow width.")]
        private float shadowWidthMultiplier = 1.0f;

        [SerializeField, Tooltip("Sorting order offset relative to caster.")]
        private int sortingOrderOffset = -1;

        [Header("System References")]
        [SerializeField] private AtmosphericLightingController lightingController;

        private SpriteRenderer casterRenderer;
        private SpriteRenderer shadowRenderer;
        private GameObject shadowGO;

        private void Start()
        {
            casterRenderer = GetComponent<SpriteRenderer>();
            if (casterRenderer == null)
            {
                casterRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (lightingController == null)
            {
                lightingController = FindAnyObjectByType<AtmosphericLightingController>();
            }

            CreateShadowObject();
        }

        private void LateUpdate()
        {
            if (casterRenderer == null || casterRenderer.sprite == null) return;

            if (shadowRenderer == null)
            {
                CreateShadowObject();
                if (shadowRenderer == null) return;
            }

            // Sync sprite frame with parent (e.g. character walking animation)
            shadowRenderer.sprite = casterRenderer.sprite;
            shadowRenderer.flipX = casterRenderer.flipX;
            shadowRenderer.flipY = casterRenderer.flipY;

            // Fetch current normalized time (0..1)
            float t = 0.5f; // Default winter afternoon
            if (lightingController != null)
            {
                t = lightingController.NormalizedTime;
            }

            UpdateShadowTransform(t);
        }

        private void CreateShadowObject()
        {
            Transform existing = transform.Find("ShadowSprite");
            if (existing != null)
            {
                shadowGO = existing.gameObject;
                shadowRenderer = shadowGO.GetComponent<SpriteRenderer>();
            }
            else
            {
                shadowGO = new GameObject("ShadowSprite");
                shadowGO.transform.SetParent(transform, false);
                shadowRenderer = shadowGO.AddComponent<SpriteRenderer>();
            }

            shadowGO.transform.localPosition = baseShadowOffset;

            if (casterRenderer != null)
            {
                shadowRenderer.sprite = casterRenderer.sprite;
                shadowRenderer.sortingLayerID = casterRenderer.sortingLayerID;
                shadowRenderer.sortingOrder = casterRenderer.sortingOrder + sortingOrderOffset;
            }

            shadowRenderer.color = shadowColor;
        }

        /// <summary>
        /// Updates the 2D shadow skew, length, angle, and opacity based on 24-hour normalized time (0..1).
        /// </summary>
        public void UpdateShadowTransform(float normalizedTime)
        {
            // Daytime: 0.25 (Dawn 06:00) -> 0.50 (Noon 12:00) -> 0.75 (Dusk 18:00)
            bool isDaytime = normalizedTime >= 0.22f && normalizedTime <= 0.78f;

            float sunYaw = (normalizedTime * 360f) - 100f; // Sun position in sky
            float shadowAngle = sunYaw + 180f;             // Shadow points opposite sun

            // Sun elevation angle: low at dawn/dusk, peak at midday
            float dayProgress = Mathf.InverseLerp(0.22f, 0.78f, normalizedTime);
            float sunPitch = Mathf.Sin(dayProgress * Mathf.PI); // 0 at dawn/dusk, 1 at midday

            // Shadow length factor: short at noon, long at morning/evening
            float lengthFactor = Mathf.Lerp(2.2f, 0.8f, sunPitch);

            // Opacity: fades gently during twilight and night
            float alpha = isDaytime ? Mathf.Lerp(0.20f, shadowColor.a, sunPitch) : 0.12f;
            Color currentShadowColor = shadowColor;
            currentShadowColor.a = alpha;
            shadowRenderer.color = currentShadowColor;

            // Apply rotation and skew scale to simulate ground shadow
            shadowGO.transform.localPosition = baseShadowOffset + new Vector3(
                -Mathf.Cos(shadowAngle * Mathf.Deg2Rad) * 0.15f * lengthFactor,
                -Mathf.Sin(shadowAngle * Mathf.Deg2Rad) * 0.10f * lengthFactor,
                0f
            );

            shadowGO.transform.localRotation = Quaternion.Euler(0f, 0f, shadowAngle - 90f);
            shadowGO.transform.localScale = new Vector3(shadowWidthMultiplier, lengthFactor, 1f);
        }
    }

    public static class ExtensionMath
    {
        public static float InverseTransformRange(float val, float min, float max)
        {
            if (max - min <= 0.0001f) return 0f;
            return Mathf.Clamp01((val - min) / (max - min));
        }
    }
}
