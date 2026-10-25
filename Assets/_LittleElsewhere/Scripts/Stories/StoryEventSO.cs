using System.Collections.Generic;
using UnityEngine;
using LittleElsewhere.Lighting;

namespace LittleElsewhere.Stories
{
    public enum SeasonRequirement
    {
        Any,
        Winter,
        Spring,
        Summer,
        Autumn
    }

    public enum TerrainRequirement
    {
        Any,
        Clearing,
        NearWater,
        NearCabin,
        DenseForest
    }

    /// <summary>
    /// Data-driven definition for a mystery or story event.
    /// Supports environmental requirements, valid seasons/times, clue chains, dependencies, and outcomes.
    /// </summary>
    [CreateAssetMenu(fileName = "NewStoryEvent", menuName = "LittleElsewhere/Story/Story Event")]
    public class StoryEventSO : ScriptableObject
    {
        [Header("Event Identity")]
        public string eventID;
        public string title;

        [Header("Narrative Description")]
        [TextArea(3, 6)]
        public string narrativeSummary;

        [Header("Environmental & Seasonal Requirements")]
        public TerrainRequirement requiredTerrain = TerrainRequirement.Any;
        public SeasonRequirement requiredSeason = SeasonRequirement.Any;
        public TimePeriod requiredTimeOfDay = TimePeriod.Afternoon;
        public bool allowAnyTimeOfDay = true;

        [Header("Story Progression & Dependencies")]
        public List<string> requiredPrerequisiteEventIDs = new List<string>();
        public bool isOneTimeEvent = true;

        [Header("Clues & Multiple Discovery Routes")]
        public List<ClueSO> primaryRouteClues = new List<ClueSO>();
        public List<ClueSO> secondaryRouteClues = new List<ClueSO>();

        [Header("Outcomes & Rewards")]
        public JournalEntrySO completionJournalEntry;
        public string achievementIDToTrigger;

        /// <summary>
        /// Checks if a discovery route is satisfied based on discovered clue IDs.
        /// </summary>
        public bool IsRouteSatisfied(HashSet<string> discoveredClueIDs)
        {
            if (primaryRouteClues != null && primaryRouteClues.Count > 0)
            {
                bool primaryComplete = true;
                foreach (var clue in primaryRouteClues)
                {
                    if (clue != null && !discoveredClueIDs.Contains(clue.clueID))
                    {
                        primaryComplete = false;
                        break;
                    }
                }
                if (primaryComplete) return true;
            }

            if (secondaryRouteClues != null && secondaryRouteClues.Count > 0)
            {
                bool secondaryComplete = true;
                foreach (var clue in secondaryRouteClues)
                {
                    if (clue != null && !discoveredClueIDs.Contains(clue.clueID))
                    {
                        secondaryComplete = false;
                        break;
                    }
                }
                if (secondaryComplete) return true;
            }

            return false;
        }
    }
}
