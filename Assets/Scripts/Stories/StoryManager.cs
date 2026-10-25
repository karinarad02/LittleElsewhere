using System.Collections.Generic;
using UnityEngine;
using LittleElsewhere.World;
using LittleElsewhere.Lighting;

namespace LittleElsewhere.Stories
{
    /// <summary>
    /// Runtime manager that monitors player proximity to placed story nodes,
    /// tracks discovered clues, evaluates multiple discovery routes, and triggers journal/achievement updates.
    /// </summary>
    public class StoryManager : MonoBehaviour
    {
        [Header("Configuration & References")]
        [SerializeField] private WorldSeedData worldConfig;
        [SerializeField] private List<StoryEventSO> storyEvents = new List<StoryEventSO>();
        [SerializeField] private Transform playerTransform;
        [SerializeField] private JournalManager journalManager;
        [SerializeField] private AchievementManager achievementManager;

        [Header("Interaction Radius")]
        [SerializeField] private float discoveryRadius = 2.5f;

        private List<PlacedStoryNode> placedNodes = new List<PlacedStoryNode>();
        private HashSet<string> discoveredClueIDs = new HashSet<string>();
        private HashSet<string> completedEventIDs = new HashSet<string>();

        public event System.Action<ClueSO> OnClueDiscovered;
        public event System.Action<StoryEventSO> OnStoryEventCompleted;

        private void Start()
        {
            if (journalManager == null) journalManager = FindAnyObjectByType<JournalManager>();
            if (achievementManager == null) achievementManager = FindAnyObjectByType<AchievementManager>();

            InitializeStoryPlacement();
        }

        public void InitializeStoryPlacement()
        {
            if (worldConfig == null) worldConfig = ScriptableObject.CreateInstance<WorldSeedData>();
            placedNodes = StoryPlacementEngine.PlaceStoryNodes(storyEvents, worldConfig);
            Debug.Log($"[StoryManager] Placed {placedNodes.Count} story & clue nodes for seed {worldConfig.worldSeed}.");
        }

        private void Update()
        {
            if (playerTransform == null) return;

            Vector2 canonicalPlayerPos = ToroidalMath.WrapPosition(playerTransform.position, worldConfig.WorldSize);

            foreach (var node in placedNodes)
            {
                if (completedEventIDs.Contains(node.storyEvent.eventID)) continue;

                float dist = ToroidalMath.WrappedDistance(canonicalPlayerPos, node.canonicalWorldPos, worldConfig.WorldSize);
                if (dist <= discoveryRadius)
                {
                    ProcessProximityDiscovery(node);
                }
            }
        }

        private void ProcessProximityDiscovery(PlacedStoryNode node)
        {
            StoryEventSO evt = node.storyEvent;

            // Check prerequisite event dependencies
            if (evt.requiredPrerequisiteEventIDs != null)
            {
                foreach (var prereqID in evt.requiredPrerequisiteEventIDs)
                {
                    if (!completedEventIDs.Contains(prereqID)) return; // Prerequisite not met
                }
            }

            // Register clue if node is a clue
            if (!node.isAnchor)
            {
                if (!discoveredClueIDs.Contains(node.nodeID))
                {
                    discoveredClueIDs.Add(node.nodeID);
                    Debug.Log($"[StoryManager] Discovered clue: {node.nodeID}");

                    // Record associated clue journal entry
                    ClueSO clueSO = FindClueByID(evt, node.nodeID);
                    if (clueSO != null)
                    {
                        if (clueSO.unlockedJournalEntry != null && journalManager != null)
                        {
                            journalManager.AddJournalEntry(clueSO.unlockedJournalEntry);
                        }
                        OnClueDiscovered?.Invoke(clueSO);
                    }
                }
            }

            // Evaluate route satisfaction for story completion
            if (evt.IsRouteSatisfied(discoveredClueIDs) || (node.isAnchor && (evt.primaryRouteClues == null || evt.primaryRouteClues.Count == 0)))
            {
                CompleteStoryEvent(evt);
            }
        }

        public void CompleteStoryEvent(StoryEventSO evt)
        {
            if (completedEventIDs.Contains(evt.eventID)) return;

            completedEventIDs.Add(evt.eventID);
            Debug.Log($"[StoryManager] Story Event Completed: '{evt.title}'");

            if (evt.completionJournalEntry != null && journalManager != null)
            {
                journalManager.AddJournalEntry(evt.completionJournalEntry);
            }

            if (!string.IsNullOrEmpty(evt.achievementIDToTrigger) && achievementManager != null)
            {
                achievementManager.UnlockAchievement(evt.achievementIDToTrigger);
            }

            OnStoryEventCompleted?.Invoke(evt);
        }

        private ClueSO FindClueByID(StoryEventSO evt, string clueID)
        {
            if (evt.primaryRouteClues != null)
            {
                foreach (var c in evt.primaryRouteClues)
                {
                    if (c != null && c.clueID == clueID) return c;
                }
            }
            if (evt.secondaryRouteClues != null)
            {
                foreach (var c in evt.secondaryRouteClues)
                {
                    if (c != null && c.clueID == clueID) return c;
                }
            }
            return null;
        }

        public bool IsEventCompleted(string eventID) => completedEventIDs.Contains(eventID);
        public bool IsClueDiscovered(string clueID) => discoveredClueIDs.Contains(clueID);
        public List<PlacedStoryNode> GetPlacedNodes() => new List<PlacedStoryNode>(placedNodes);
    }
}
