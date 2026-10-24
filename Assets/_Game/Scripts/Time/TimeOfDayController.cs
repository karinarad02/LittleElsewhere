using UnityEngine;
using LittleElsewhere.Lighting;

namespace LittleElsewhere.TimeSystem
{
    /// <summary>
    /// Clock controller that advances in-game time (0..24 hrs) and feeds progress into AtmosphericLightingController.
    /// Configurable day length (default: 48 real minutes per in-game day).
    /// </summary>
    public class TimeOfDayController : MonoBehaviour
    {
        [Header("Clock Settings")]
        [SerializeField, Tooltip("Real-world seconds for one full in-game day (e.g. 2880s = 48 minutes).")]
        private float dayDurationSeconds = 2880f;

        [SerializeField, Range(0f, 24f), Tooltip("Current in-game time in hours (0.0 to 24.0).")]
        private float currentHour = 14.5f; // Benchmark: 2:30 PM Winter Afternoon

        [SerializeField, Tooltip("Pause time progression.")]
        private bool pauseClock = false;

        [Header("Lighting Reference")]
        [SerializeField]
        private AtmosphericLightingController lightingController;

        public float CurrentHour => currentHour;
        public float NormalizedTime => currentHour / 24.0f;

        private void Start()
        {
            if (lightingController == null)
            {
                lightingController = GetComponent<AtmosphericLightingController>();
            }
            if (lightingController == null)
            {
                lightingController = FindFirstObjectByType<AtmosphericLightingController>();
            }
        }

        private void Update()
        {
            if (!pauseClock && dayDurationSeconds > 0f)
            {
                currentHour += (Time.deltaTime / dayDurationSeconds) * 24.0f;
                if (currentHour >= 24.0f)
                {
                    currentHour -= 24.0f;
                }
            }

            if (lightingController != null)
            {
                lightingController.SetNormalizedTime(NormalizedTime);
            }
        }

        public void SetHour(float hour)
        {
            currentHour = Mathf.Repeat(hour, 24.0f);
            if (lightingController != null)
            {
                lightingController.SetNormalizedTime(NormalizedTime);
            }
        }
    }
}
