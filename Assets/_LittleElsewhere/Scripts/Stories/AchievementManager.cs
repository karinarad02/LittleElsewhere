using System.Collections.Generic;
using UnityEngine;

namespace LittleElsewhere.Stories
{
    /// <summary>
    /// Event-driven achievement unlocker for exploration and discovery milestones.
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        private HashSet<string> unlockedAchievements = new HashSet<string>();

        public event System.Action<string> OnAchievementUnlocked;

        public bool UnlockAchievement(string achievementID)
        {
            if (string.IsNullOrEmpty(achievementID) || unlockedAchievements.Contains(achievementID))
            {
                return false;
            }

            unlockedAchievements.Add(achievementID);
            Debug.Log($"[AchievementManager] Achievement Unlocked: {achievementID}");
            OnAchievementUnlocked?.Invoke(achievementID);
            return true;
        }

        public bool IsUnlocked(string achievementID)
        {
            return unlockedAchievements.Contains(achievementID);
        }
    }
}
