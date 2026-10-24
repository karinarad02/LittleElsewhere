using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using LittleElsewhere.Stories;
using LittleElsewhere.World;

namespace LittleElsewhere.Tests
{
    [TestFixture]
    public class StorySystemTests
    {
        private StoryEventSO mysteryEvent;
        private ClueSO clueLakeTracks;
        private ClueSO clueBridgeJournal;
        private ClueSO clueNeighborRumor;
        private JournalEntrySO completionJournalEntry;

        [SetUp]
        public void SetUp()
        {
            // Vertical Slice Sample Mystery: "The Whispering Cabin of the North Lake"
            completionJournalEntry = ScriptableObject.CreateInstance<JournalEntrySO>();
            completionJournalEntry.entryID = "J_WHISPERING_CABIN";
            completionJournalEntry.title = "The Whispering Cabin Memory";

            clueLakeTracks = ScriptableObject.CreateInstance<ClueSO>();
            clueLakeTracks.clueID = "C_LAKE_TRACKS";
            clueLakeTracks.clueName = "Strange Footprints near North Lake";

            clueBridgeJournal = ScriptableObject.CreateInstance<ClueSO>();
            clueBridgeJournal.clueID = "C_BRIDGE_JOURNAL";
            clueBridgeJournal.clueName = "Abandoned Journal under Old Bridge";

            clueNeighborRumor = ScriptableObject.CreateInstance<ClueSO>();
            clueNeighborRumor.clueID = "C_NEIGHBOR_RUMOR";
            clueNeighborRumor.clueName = "Neighbor's Tale of Lights across the Ice";

            mysteryEvent = ScriptableObject.CreateInstance<StoryEventSO>();
            mysteryEvent.eventID = "E_WHISPERING_CABIN";
            mysteryEvent.title = "The Whispering Cabin of the North Lake";
            mysteryEvent.requiredTerrain = TerrainRequirement.NearWater;
            mysteryEvent.completionJournalEntry = completionJournalEntry;
            mysteryEvent.achievementIDToTrigger = "ACH_CABIN_MYSTERY";

            // Route A: Direct discovery via Lake Tracks
            mysteryEvent.primaryRouteClues = new List<ClueSO> { clueLakeTracks };

            // Route B: Indirect discovery via Neighbor Rumor + Bridge Journal
            mysteryEvent.secondaryRouteClues = new List<ClueSO> { clueNeighborRumor, clueBridgeJournal };
        }

        [Test]
        public void Test_RouteA_PrimaryRouteCompletion()
        {
            HashSet<string> discoveredClues = new HashSet<string> { "C_LAKE_TRACKS" };
            bool isSatisfied = mysteryEvent.IsRouteSatisfied(discoveredClues);
            Assert.IsTrue(isSatisfied, "Primary Route A should be satisfied when Lake Tracks clue is discovered.");
        }

        [Test]
        public void Test_RouteB_SecondaryRouteCompletion()
        {
            HashSet<string> partialClues = new HashSet<string> { "C_NEIGHBOR_RUMOR" };
            Assert.IsFalse(mysteryEvent.IsRouteSatisfied(partialClues), "Route B should require both Neighbor Rumor and Bridge Journal.");

            HashSet<string> fullRouteBClues = new HashSet<string> { "C_NEIGHBOR_RUMOR", "C_BRIDGE_JOURNAL" };
            Assert.IsTrue(mysteryEvent.IsRouteSatisfied(fullRouteBClues), "Secondary Route B should be satisfied when both clues are discovered.");
        }

        [Test]
        public void Test_DeterministicStoryPlacementAcrossSeeds()
        {
            WorldSeedData configA = ScriptableObject.CreateInstance<WorldSeedData>();
            configA.worldSeed = 1337;

            WorldSeedData configB = ScriptableObject.CreateInstance<WorldSeedData>();
            configB.worldSeed = 1337;

            List<StoryEventSO> events = new List<StoryEventSO> { mysteryEvent };

            List<PlacedStoryNode> nodesA = StoryPlacementEngine.PlaceStoryNodes(events, configA);
            List<PlacedStoryNode> nodesB = StoryPlacementEngine.PlaceStoryNodes(events, configB);

            Assert.AreEqual(nodesA.Count, nodesB.Count);
            Assert.AreEqual(nodesA[0].canonicalWorldPos, nodesB[0].canonicalWorldPos, "Story placement must be deterministic for identical seeds.");
        }

        [Test]
        public void Test_JournalManager_RecordsMemoriesWithoutExposingScores()
        {
            GameObject managerGO = new GameObject();
            JournalManager journalManager = managerGO.AddComponent<JournalManager>();

            bool added = journalManager.AddJournalEntry(completionJournalEntry);
            Assert.IsTrue(added);
            Assert.IsTrue(journalManager.IsEntryUnlocked("J_WHISPERING_CABIN"));

            // Duplicate addition should be ignored
            bool addedDuplicate = journalManager.AddJournalEntry(completionJournalEntry);
            Assert.IsFalse(addedDuplicate);

            Object.DestroyImmediate(managerGO);
        }
    }
}
