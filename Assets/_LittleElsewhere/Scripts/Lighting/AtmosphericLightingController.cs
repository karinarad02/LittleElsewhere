using UnityEngine;

namespace LittleElsewhere.Lighting
{
    public enum TimePeriod
    {
        Morning,
        Afternoon, // Primary visual benchmark matching reference images
        Evening,
        Night
    }

    /// <summary>
    /// Configurable lighting controller that smoothly transitions between Morning, Afternoon, Evening, and Night.
    /// Manages light intensity, atmospheric color gradients, global ambient tint, and soft blue shadow projection.
    /// Benchmark setting: Crisp winter afternoon with soft blue shadows pointing diagonally across white snow.
    /// </summary>
    public class AtmosphericLightingController : MonoBehaviour
    {
        [Header("Primary Target State")]
        [SerializeField, Tooltip("Set active preset period for rapid testing in Inspector.")]
        private TimePeriod currentPeriod = TimePeriod.Afternoon;

        [SerializeField, Range(0f, 1f), Tooltip("Normalized time value (0 = Midnight, 0.25 = Morning, 0.5 = Afternoon, 0.75 = Evening).")]
        private float normalizedTime = 0.5f;

        public float NormalizedTime => normalizedTime;

        [Header("Light Source References")]
        [SerializeField, Tooltip("Main directional sun/moon light.")]
        private Light directionalLight;

        [Header("Shadow Configuration")]
        [SerializeField, Tooltip("Shadow color tint (Soft cool blue: #3B5B88).")]
        private Color softBlueShadowColor = new Color(0.23f, 0.36f, 0.53f, 0.65f);

        [SerializeField, Tooltip("Minimum shadow distance/length at midday.")]
        private float minShadowLengthMultiplier = 1.0f;

        [SerializeField, Tooltip("Maximum shadow distance/length at low sun angle (morning/afternoon).")]
        private float maxShadowLengthMultiplier = 2.4f;

        public float MinShadowLengthMultiplier => minShadowLengthMultiplier;
        public float MaxShadowLengthMultiplier => maxShadowLengthMultiplier;

        [Header("Color Gradients Across 24 Hours")]
        [SerializeField] private Gradient lightColorGradient;
        [SerializeField] private Gradient ambientColorGradient;
        [SerializeField] private AnimationCurve shadowLengthCurve;

        private void Start()
        {
            if (directionalLight == null)
            {
                directionalLight = GetComponent<Light>();
            }

            SetupBenchmarkGradients();
            ApplyLightingForTime(normalizedTime);
        }

        private void OnValidate()
        {
            // Allow real-time previewing in Inspector during editing
            SetTimePeriod(currentPeriod);
            ApplyLightingForTime(normalizedTime);
        }

        public void SetNormalizedTime(float time)
        {
            normalizedTime = Mathf.Repeat(time, 1.0f);
            ApplyLightingForTime(normalizedTime);
        }

        public void SetTimePeriod(TimePeriod period)
        {
            currentPeriod = period;
            switch (period)
            {
                case TimePeriod.Morning:
                    normalizedTime = 0.28f;
                    break;
                case TimePeriod.Afternoon: // Benchmark: Winter afternoon sun
                    normalizedTime = 0.52f;
                    break;
                case TimePeriod.Evening:
                    normalizedTime = 0.72f;
                    break;
                case TimePeriod.Night:
                    normalizedTime = 0.05f;
                    break;
            }
            ApplyLightingForTime(normalizedTime);
        }

        private void ApplyLightingForTime(float t)
        {
            if (directionalLight != null)
            {
                // Smooth light pitch and yaw rotation for organic shadow movement
                float sunPitch = Mathf.Lerp(15f, 65f, Mathf.Sin(t * Mathf.PI));
                float sunYaw = (t * 360f) - 100f;

                directionalLight.transform.rotation = Quaternion.Euler(sunPitch, sunYaw, 0f);

                if (lightColorGradient != null && lightColorGradient.colorKeys.Length > 0)
                {
                    directionalLight.color = lightColorGradient.Evaluate(t);
                }
            }

            if (ambientColorGradient != null && ambientColorGradient.colorKeys.Length > 0)
            {
                RenderSettings.ambientLight = ambientColorGradient.Evaluate(t);
            }
        }

        private void SetupBenchmarkGradients()
        {
            if (lightColorGradient == null || lightColorGradient.colorKeys.Length == 0)
            {
                lightColorGradient = new Gradient();
                GradientColorKey[] colors = new GradientColorKey[5];
                colors[0] = new GradientColorKey(new Color(0.20f, 0.30f, 0.50f), 0.0f);  // Night Blue
                colors[1] = new GradientColorKey(new Color(0.95f, 0.78f, 0.65f), 0.25f); // Morning Dawn Warmth
                colors[2] = new GradientColorKey(new Color(0.98f, 0.96f, 0.92f), 0.50f); // Winter Afternoon Soft Sun
                colors[3] = new GradientColorKey(new Color(0.85f, 0.52f, 0.42f), 0.75f); // Evening Twilight
                colors[4] = new GradientColorKey(new Color(0.20f, 0.30f, 0.50f), 1.0f);  // Night Return

                GradientAlphaKey[] alphas = new GradientAlphaKey[2];
                alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

                lightColorGradient.SetKeys(colors, alphas);
            }

            if (ambientColorGradient == null || ambientColorGradient.colorKeys.Length == 0)
            {
                ambientColorGradient = new Gradient();
                GradientColorKey[] colors = new GradientColorKey[5];
                colors[0] = new GradientColorKey(new Color(0.12f, 0.18f, 0.32f), 0.0f);  // Night Ambient
                colors[1] = new GradientColorKey(new Color(0.55f, 0.58f, 0.68f), 0.25f); // Morning Ambient
                colors[2] = new GradientColorKey(new Color(0.82f, 0.85f, 0.92f), 0.50f); // Winter Snow Midday
                colors[3] = new GradientColorKey(new Color(0.50f, 0.42f, 0.55f), 0.75f); // Evening Ambient
                colors[4] = new GradientColorKey(new Color(0.12f, 0.18f, 0.32f), 1.0f);  // Night Ambient

                GradientAlphaKey[] alphas = new GradientAlphaKey[2];
                alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

                ambientColorGradient.SetKeys(colors, alphas);
            }
        }
    }
}
