using UnityEngine;
using LittleElsewhere.World;

namespace LittleElsewhere.Navigation
{
    /// <summary>
    /// Pathing and navigation helper supporting toroidal (wraparound) distance & direction calculations.
    /// Used for "Find Your Way Home" environmental guidance across world edges.
    /// </summary>
    public static class WrappedPathfinding
    {
        /// <summary>
        /// Calculates shortest wrapped vector pointing from 'currentPos' towards 'targetPos' on toroidal world bounds.
        /// </summary>
        public static Vector2 GetWrappedGuidanceDirection(Vector2 currentPos, Vector2 targetPos, Vector2 worldSize)
        {
            Vector2 delta = ToroidalMath.WrappedDelta(currentPos, targetPos, worldSize);
            return delta.normalized;
        }

        /// <summary>
        /// Calculates wrapped distance between two canonical world points.
        /// </summary>
        public static float GetWrappedDistance(Vector2 currentPos, Vector2 targetPos, Vector2 worldSize)
        {
            return ToroidalMath.WrappedDistance(currentPos, targetPos, worldSize);
        }
    }
}
