using UnityEngine;
using LittleElsewhere.TimeSystem;

namespace LittleElsewhere.Environment
{
    public enum RestLocationType
    {
        PlayerCabin,
        ForestShelter,
        Campsite,
        NeighbourHome
    }

    /// <summary>
    /// Interactive rest location. Allows optional resting to advance time to a chosen wake-up hour.
    /// </summary>
    public class RestLocation : MonoBehaviour
    {
        [Header("Rest Location Metadata")]
        [SerializeField] private string locationName = "Player's Cabin";
        [SerializeField] private RestLocationType locationType = RestLocationType.PlayerCabin;

        [Header("Interaction Radius")]
        [SerializeField] private float interactionRadius = 2.0f;

        [SerializeField] private CalendarManager calendarManager;

        private void Start()
        {
            if (calendarManager == null)
            {
                calendarManager = FindFirstObjectByType<CalendarManager>();
            }
        }

        public void TriggerRest(float targetWakeUpHour)
        {
            if (calendarManager != null)
            {
                calendarManager.RestAndWakeUp(targetWakeUpHour);
                Debug.Log($"[RestLocation] Rested at '{locationName}' ({locationType}). Woke up at {targetWakeUpHour:F1}h.");
            }
        }
    }
}
