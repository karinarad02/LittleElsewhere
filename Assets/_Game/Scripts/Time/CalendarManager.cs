using UnityEngine;
using LittleElsewhere.Lighting;

namespace LittleElsewhere.TimeSystem
{
    public enum Season
    {
        Winter,
        Spring,
        Summer,
        Autumn
    }

    /// <summary>
    /// Manages accelerated 24-hour day/night cycle, day counter, and 4-season calendar progression.
    /// Default setup: 48 real minutes per day, 12 in-game days per season.
    /// Supports optional resting/time-skipping without survival penalties.
    /// </summary>
    public class CalendarManager : MonoBehaviour
    {
        [Header("Time & Calendar Settings")]
        [SerializeField, Tooltip("Duration of one in-game day in real-world seconds (default: 2880s = 48 minutes).")]
        private float dayDurationSeconds = 2880f;

        [SerializeField, Tooltip("Number of in-game days per season.")]
        private int daysPerSeason = 12;

        [SerializeField, Range(0f, 24f)]
        private float currentHour = 8.0f; // Start at 8 AM

        [SerializeField] private int currentDayOfSeason = 1;
        [SerializeField] private Season currentSeason = Season.Winter;
        [SerializeField] private int currentYear = 1;

        [Header("Lighting Reference")]
        [SerializeField] private AtmosphericLightingController lightingController;

        public float CurrentHour => currentHour;
        public float NormalizedTime => currentHour / 24.0f;
        public int CurrentDayOfSeason => currentDayOfSeason;
        public Season CurrentSeason => currentSeason;
        public int CurrentYear => currentYear;

        public event System.Action<float> OnHourChanged;
        public event System.Action<int, Season> OnDayChanged;
        public event System.Action<Season, int> OnSeasonChanged;

        private void Start()
        {
            if (lightingController == null)
            {
                lightingController = FindFirstObjectByType<AtmosphericLightingController>();
            }
        }

        private void Update()
        {
            if (dayDurationSeconds <= 0f) return;

            float previousHour = currentHour;
            currentHour += (Time.deltaTime / dayDurationSeconds) * 24.0f;

            if (Mathf.FloorToInt(previousHour) != Mathf.FloorToInt(currentHour))
            {
                OnHourChanged?.Invoke(currentHour);
            }

            if (currentHour >= 24.0f)
            {
                currentHour -= 24.0f;
                AdvanceDay();
            }

            if (lightingController != null)
            {
                lightingController.SetNormalizedTime(NormalizedTime);
            }
        }

        public void AdvanceDay()
        {
            currentDayOfSeason++;
            if (currentDayOfSeason > daysPerSeason)
            {
                currentDayOfSeason = 1;
                AdvanceSeason();
            }

            OnDayChanged?.Invoke(currentDayOfSeason, currentSeason);
        }

        public void AdvanceSeason()
        {
            currentSeason = (Season)(((int)currentSeason + 1) % 4);
            if (currentSeason == Season.Winter)
            {
                currentYear++;
            }

            Debug.Log($"[CalendarManager] Season Changed: {currentSeason}, Year {currentYear}");
            OnSeasonChanged?.Invoke(currentSeason, currentYear);
        }

        /// <summary>
        /// Rest and fast-forward time to a chosen wake-up hour (e.g. 6.0 = Dawn, 12.0 = Midday).
        /// </summary>
        public void RestAndWakeUp(float targetWakeUpHour)
        {
            targetWakeUpHour = Mathf.Repeat(targetWakeUpHour, 24.0f);

            if (targetWakeUpHour <= currentHour)
            {
                // Wake up is next day
                AdvanceDay();
            }

            currentHour = targetWakeUpHour;

            if (lightingController != null)
            {
                lightingController.SetNormalizedTime(NormalizedTime);
            }

            OnHourChanged?.Invoke(currentHour);
            Debug.Log($"[CalendarManager] Player rested and woke up at {currentHour:F1}h. Season: {currentSeason}, Day: {currentDayOfSeason}");
        }

        public void SetCalendarState(float hour, int day, Season season, int year)
        {
            currentHour = Mathf.Repeat(hour, 24.0f);
            currentDayOfSeason = Mathf.Clamp(day, 1, daysPerSeason);
            currentSeason = season;
            currentYear = Mathf.Max(1, year);

            if (lightingController != null)
            {
                lightingController.SetNormalizedTime(NormalizedTime);
            }
        }
    }
}
