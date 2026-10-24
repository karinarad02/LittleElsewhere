using System.Collections.Generic;
using UnityEngine;

namespace LittleElsewhere.Stories
{
    public enum EndingCategory
    {
        AnimalLover,
        NeighbourCommunity,
        SupernaturalDiscovery,
        PeacefulExplorer,
        MixedHarmony
    }

    [System.Serializable]
    public class EndingDefinition
    {
        public string endingID;
        public string endingTitle;
        public EndingCategory category;
        [TextArea(4, 8)] public string epilogueText;
        public int requiredDiscoveryCount = 3;
    }

    /// <summary>
    /// Flexible ending engine. Evaluates player's accumulated memories organically
    /// across narrative categories (Wildlife, Community, Supernatural, Exploration).
    /// Supports mixed-interest endings and allows post-ending free exploration.
    /// </summary>
    public class EndingManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private JournalManager journalManager;
        [SerializeField] private AchievementManager achievementManager;

        [Header("Ending Definitions")]
        [SerializeField] private List<EndingDefinition> endings = new List<EndingDefinition>();

        private HashSet<string> unlockedEndingIDs = new HashSet<string>();

        public event System.Action<EndingDefinition> OnEndingUnlocked;

        private void Start()
        {
            if (journalManager == null) journalManager = FindFirstObjectByType<JournalManager>();
            if (achievementManager == null) achievementManager = FindFirstObjectByType<AchievementManager>();

            SetupDefaultEndingsIfEmpty();
        }

        public void EvaluateEndings()
        {
            if (journalManager == null) return;

            List<JournalEntrySO> entries = journalManager.GetAllEntries();
            int wildlifeCount = 0;
            int neighbourCount = 0;
            int supernaturalCount = 0;
            int totalCount = entries.Count;

            foreach (var entry in entries)
            {
                if (entry.category == JournalCategory.Wildlife) wildlifeCount++;
                else if (entry.category == JournalCategory.Neighbours) neighbourCount++;
                else if (entry.category == JournalCategory.Supernatural) supernaturalCount++;
            }

            foreach (var ending in endings)
            {
                if (unlockedEndingIDs.Contains(ending.endingID)) continue;

                bool conditionMet = false;
                switch (ending.category)
                {
                    case EndingCategory.AnimalLover:
                        conditionMet = (wildlifeCount >= ending.requiredDiscoveryCount);
                        break;
                    case EndingCategory.NeighbourCommunity:
                        conditionMet = (neighbourCount >= ending.requiredDiscoveryCount);
                        break;
                    case EndingCategory.SupernaturalDiscovery:
                        conditionMet = (supernaturalCount >= ending.requiredDiscoveryCount);
                        break;
                    case EndingCategory.PeacefulExplorer:
                        conditionMet = (totalCount >= ending.requiredDiscoveryCount);
                        break;
                    case EndingCategory.MixedHarmony:
                        conditionMet = (wildlifeCount >= 1 && neighbourCount >= 1 && supernaturalCount >= 1);
                        break;
                }

                if (conditionMet)
                {
                    TriggerEnding(ending);
                }
            }
        }

        private void TriggerEnding(EndingDefinition ending)
        {
            unlockedEndingIDs.Add(ending.endingID);
            Debug.Log($"[EndingManager] Ending Unlocked: '{ending.endingTitle}' ({ending.category})");

            if (achievementManager != null)
            {
                achievementManager.UnlockAchievement($"ACH_ENDING_{ending.endingID}");
            }

            OnEndingUnlocked?.Invoke(ending);
        }

        private void SetupDefaultEndingsIfEmpty()
        {
            if (endings.Count == 0)
            {
                endings.Add(new EndingDefinition
                {
                    endingID = "END_ANIMAL",
                    endingTitle = "Guardian of the Forest Creatures",
                    category = EndingCategory.AnimalLover,
                    epilogueText = "Through quiet observation, you formed a deep bond with the forest's wildlife.",
                    requiredDiscoveryCount = 3
                });

                endings.Add(new EndingDefinition
                {
                    endingID = "END_COMMUNITY",
                    endingTitle = "A Warm Hearth Among Neighbours",
                    category = EndingCategory.NeighbourCommunity,
                    epilogueText = "Your gentle visits brought warmth and connection to the lonely forest cabins.",
                    requiredDiscoveryCount = 3
                });

                endings.Add(new EndingDefinition
                {
                    endingID = "END_HARMONY",
                    endingTitle = "Harmony in the Woods",
                    category = EndingCategory.MixedHarmony,
                    epilogueText = "You embraced every aspect of the forest, weaving a tapestry of stories, nature, and friendship.",
                    requiredDiscoveryCount = 3
                });
            }
        }

        public bool IsEndingUnlocked(string endingID) => unlockedEndingIDs.Contains(endingID);
    }
}
