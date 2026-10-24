using NUnit.Framework;
using UnityEngine;
using LittleElsewhere.Navigation;
using LittleElsewhere.Stories;
using LittleElsewhere.World;

namespace LittleElsewhere.Tests
{
    [TestFixture]
    public class NavigationJournalEndingTests
    {
        private GameObject testGO;
        private NavigationManager navManager;
        private JournalManager journalManager;
        private EndingManager endingManager;

        [SetUp]
        public void SetUp()
        {
            testGO = new GameObject();
            navManager = testGO.AddComponent<NavigationManager>();
            journalManager = testGO.AddComponent<JournalManager>();
            endingManager = testGO.AddComponent<EndingManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (testGO != null) Object.DestroyImmediate(testGO);
        }

        [Test]
        public void Test_GetFindWayHomeVector_CalculatesShortestWrappedVector()
        {
            Vector2 playerPos = new Vector2(150f, 24f); // Near East boundary
            Vector2 cabinPos = new Vector2(10f, 24f);   // Near West boundary
            Vector2 worldSize = new Vector2(160f, 160f);

            Vector2 delta = ToroidalMath.WrappedDelta(playerPos, cabinPos, worldSize);
            Assert.AreEqual(20f, delta.x, 0.001f, "Shortest path to cabin across wraparound should be east (+20).");
        }

        [Test]
        public void Test_EndingEvaluation_UnlocksAnimalLoverEnding()
        {
            // Add 3 Wildlife journal entries
            for (int i = 1; i <= 3; i++)
            {
                JournalEntrySO entry = ScriptableObject.CreateInstance<JournalEntrySO>();
                entry.entryID = $"J_WILDLIFE_{i}";
                entry.title = $"Wildlife Observation {i}";
                entry.category = JournalCategory.Wildlife;
                journalManager.AddJournalEntry(entry);
            }

            endingManager.EvaluateEndings();
            Assert.IsTrue(endingManager.IsEndingUnlocked("END_ANIMAL"), "AnimalLover ending should be unlocked after 3 wildlife discoveries.");
        }

        [Test]
        public void Test_EndingEvaluation_UnlocksMixedHarmonyEnding()
        {
            // Add 1 Wildlife, 1 Neighbour, 1 Supernatural entry
            JournalEntrySO entry1 = ScriptableObject.CreateInstance<JournalEntrySO>();
            entry1.entryID = "J_W1"; entry1.category = JournalCategory.Wildlife;
            journalManager.AddJournalEntry(entry1);

            JournalEntrySO entry2 = ScriptableObject.CreateInstance<JournalEntrySO>();
            entry2.entryID = "J_N1"; entry2.category = JournalCategory.Neighbours;
            journalManager.AddJournalEntry(entry2);

            JournalEntrySO entry3 = ScriptableObject.CreateInstance<JournalEntrySO>();
            entry3.entryID = "J_S1"; entry3.category = JournalCategory.Supernatural;
            journalManager.AddJournalEntry(entry3);

            endingManager.EvaluateEndings();
            Assert.IsTrue(endingManager.IsEndingUnlocked("END_HARMONY"), "Mixed Harmony ending should be unlocked when player engages with all story categories.");
        }
    }
}
