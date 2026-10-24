using UnityEngine;

namespace LittleElsewhere.Environment
{
    /// <summary>
    /// Manages the 24-hour day/night cycle, atmospheric URP lighting transitions, and long soft blue shadows.
    /// Configurable for real-time to in-game time scaling (e.g., 48 real minutes per in-game day).
    /// </summary>
    public class DayNightCycle : MonoBehaviour
    {
        [Header("Time Settings")]
        [SerializeField, Tooltip("Duration of an in-game day in real-world seconds (e.g. 2880s = 48 minutes).")]
        private float dayDurationInSeconds = 2880f;

        [SerializeField, Range(0f, 24f), Tooltip("Current time of day in 24-hour format.")]
        private float currentTimeOfDay = 8.0f; // Start at 8:00 AM winter morning

        [SerializeField, Tooltip("Freeze time progression for testing.")]
        private bool freezeTime = false;

        [Header("Lighting References")]
        [SerializeField, Tooltip("Main directional sun/moon light source.")]
        private Light directionalLight;

        [Header("Color Gradients")]
        [SerializeField, Tooltip("Light color across the 24-hour day (Dawn, Noon, Dusk, Night).")]
        private Gradient lightColorGradient;

        [SerializeField, Tooltip("Ambient light color across the 24-hour day.")]
        private Gradient ambientColorGradient;

        [Header("Shadow Configuration")]
        [SerializeField, Tooltip("Color of tree and environmental shadows (Soft blue for painterly winter look).")]
        private Color winterShadowColor = new Color(0.2f, 0.35f, 0.65f, 0.4f);

        public float CurrentTimeOfDay => currentTimeOfDay;
        public float NormalizedTime => currentTimeOfDay / 24.0f;

        private void Start()
        {
            if (directionalLight == null)
            {
                directionalLight = GetComponent<Light>();
            }

            SetupDefaultGradientsIfNull();
        }

        private void Update()
        {
            if (!freezeTime)
            {
                currentTimeOfDay += (Time.deltaTime / dayDurationInSeconds) * 24.0f;
                if (currentTimeOfDay >= 24.0f)
                {
                    currentTimeOfDay -= 24.0f;
                }
            }

            UpdateLighting();
        }

        private void UpdateLighting()
        {
            float timePercent = NormalizedTime;

            // Rotate directional light to change shadow length and angle throughout the day
            float sunAngle = (timePercent * 360f) - 90f;
            if (directionalLight != null)
            {
                directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, 30f, 0f));
                directionalLight.color = lightColorGradient.Evaluate(timePercent);
            }

            RenderSettings.ambientLight = ambientColorGradient.Evaluate(timePercent);
        }

        private void SetupDefaultGradientsIfNull()
        {
            if (lightColorGradient == null || lightColorGradient.colorKeys.Length == 0)
            {
                lightColorGradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[5];
                // 0h Night, 6h Dawn, 12h Noon, 18h Dusk, 24h Night
                colorKeys[0] = new GradientColorKey(new Color(0.25f, 0.35f, 0.55f), 0.0f); // Soft Night Blue
                colorKeys[1] = new GradientColorKey(new Color(0.95f, 0.75f, 0.60f), 0.25f); // Golden Dawn
                colorKeys[2] = new GradientColorKey(new Color(0.98f, 0.96f, 0.90f), 0.50f); // Soft Midday Winter Sun
                colorKeys[3] = new GradientColorKey(new Color(0.85f, 0.55f, 0.45f), 0.75f); // Atmospheric Dusk
                colorKeys[4] = new GradientColorKey(new Color(0.25f, 0.35f, 0.55f), 1.00f); // Return to Night

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

                lightColorGradient.SetKeys(colorKeys, alphaKeys);
            }

            if (ambientColorGradient == null || ambientColorGradient.colorKeys.Length == 0)
            {
                ambientColorGradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[5];
                colorKeys[0] = new GradientColorKey(new Color(0.15f, 0.22f, 0.38f), 0.0f); // Night Ambient
                colorKeys[1] = new GradientColorKey(new Color(0.45f, 0.48f, 0.58f), 0.25f); // Dawn Ambient
                colorKeys[2] = new GradientColorKey(new Color(0.70f, 0.75f, 0.85f), 0.50f); // Winter Snow Ambient
                colorKeys[3] = new GradientColorKey(new Color(0.48f, 0.42f, 0.52f), 0.75f); // Dusk Ambient
                colorKeys[4] = new GradientColorKey(new Color(0.15f, 0.22f, 0.38f), 1.00f); // Night Ambient

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
                alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

                ambientColorGradient.SetKeys(colorKeys, alphaKeys);
            }
        }
    }
}
