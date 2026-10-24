using System.Collections.Generic;
using UnityEngine;
using LittleElsewhere.World;
using LittleElsewhere.Environment;

namespace LittleElsewhere.Navigation
{
    /// <summary>
    /// Manages wraparound navigation guidance ("Find Your Way Home"), direct fast travel ("Return Home"),
    /// and registered rest locations.
    /// </summary>
    public class NavigationManager : MonoBehaviour
    {
        [Header("Home & World References")]
        [SerializeField] private WorldSeedData worldConfig;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Vector2 cabinCanonicalPosition = new Vector2(24f, 24f);

        [Header("Discovered Rest Locations")]
        private List<RestLocation> discoveredRestLocations = new List<RestLocation>();

        public Vector2 CabinCanonicalPosition => cabinCanonicalPosition;

        public event System.Action OnReturnHomeTransitionStarted;
        public event System.Action OnReturnHomeTransitionCompleted;

        private void Start()
        {
            if (worldConfig == null) worldConfig = ScriptableObject.CreateInstance<WorldSeedData>();
            if (playerTransform == null)
            {
                GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null) playerTransform = playerGO.transform;
            }
        }

        /// <summary>
        /// Calculates subtle directional vector pointing towards cabin considering toroidal wraparound bounds.
        /// </summary>
        public Vector2 GetFindWayHomeVector()
        {
            if (playerTransform == null || worldConfig == null) return Vector2.zero;

            Vector2 canonicalPlayerPos = ToroidalMath.WrapPosition(playerTransform.position, worldConfig.WorldSize);
            return ToroidalMath.WrappedDelta(canonicalPlayerPos, cabinCanonicalPosition, worldConfig.WorldSize).normalized;
        }

        /// <summary>
        /// Direct travel to home cabin with gentle transition.
        /// </summary>
        public void ReturnHome()
        {
            if (playerTransform == null) return;

            OnReturnHomeTransitionStarted?.Invoke();

            // Teleport player directly to cabin canonical coordinate
            playerTransform.position = cabinCanonicalPosition;

            Debug.Log($"[NavigationManager] Player returned home to cabin at {cabinCanonicalPosition}.");
            OnReturnHomeTransitionCompleted?.Invoke();
        }

        public void RegisterDiscoveredRestLocation(RestLocation location)
        {
            if (location != null && !discoveredRestLocations.Contains(location))
            {
                discoveredRestLocations.Add(location);
                Debug.Log($"[NavigationManager] Discovered rest location registered: '{location.name}'");
            }
        }

        public List<RestLocation> GetDiscoveredRestLocations() => new List<RestLocation>(discoveredRestLocations);
    }
}
